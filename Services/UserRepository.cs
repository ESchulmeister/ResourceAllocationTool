using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using System.Data.SqlClient;


namespace ResourceAllocationTool.Services
{
    public class UserRepository : IUserRepository
    {
        #region Variables

    private readonly TechCMSContext _context;
        private string _sLoginName;

        #endregion

        #region constructors

        public UserRepository(TechCMSContext context, IHttpContextAccessor oHttpContextAccessor)
        {
           _context = context;
           _sLoginName = oHttpContextAccessor.GetIdentity();
        }
        #endregion

        #region Methods
        /// <summary>
        /// List active users, including role records
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> ListAllAsync()
        {
            IQueryable<User> query = _context.Users
                            .Where(e => e.UsrActive.HasValue && e.UsrActive.Value == 1)
                            .Include(x => x.UsrDefaultRoleNavigation)
                            .AsNoTracking();

            return await query.OrderBy(oUser => oUser.UsrLast).ThenBy(oUser => oUser.UsrFirst).ToListAsync();
        }

        /// <summary>
        /// List only tracked (hour )  users - USR_APPS = 16
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> ListTrackedAsync()
        {
            IQueryable<User> query = _context.Users
                            .Where
                            (
                                e => e.UsrActive.HasValue && e.UsrActive.Value == 1 &&
                                (e.UsrApps.HasValue && (e.UsrApps.Value & Constants.ApplicationBit) == Constants.ApplicationBit)
                            )
                            .Include(x => x.UsrDefaultRoleNavigation)
                            .AsNoTracking();

            return await query.OrderBy(oUser => oUser.UsrLast).ThenBy(oUser => oUser.UsrFirst).ToListAsync();
        }

        /// <summary>
        /// Users By Project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns>JSON ProjectUser array, w/o supervisors</returns>
        public async Task<IEnumerable<ProjectUser>> ListByProjectAsync(int projectID)
        {
            IQueryable<ProjectUser> query =
                _context.ProjectUsers.Where(e => e.PuProjectId == projectID && e.PuRoleId != Constants.Roles.Supervisor && e.PuActive == true);

            return await query.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Available - Only Track Hour ones
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> ListAvailableAsync(int projectID)
        {
            var projectUsers =
                from pu in _context.ProjectUsers
                where pu.PuProjectId == projectID
                select new
                {
                    userID = pu.PuUserId
                };

            IQueryable<User> query =
                from u in _context.Users
                where !projectUsers.Any(pu => pu.userID == u.UsrId)
                where (u.UsrApps & Constants.ApplicationBit) == Constants.ApplicationBit
                select u;

            return await query.AsNoTracking().OrderBy(oUser => oUser.UsrLast).ThenBy(oUser => oUser.UsrFirst).ToListAsync();
        }

        /// <summary>
        /// Load By Login
        /// </summary>
        /// <param name="sLogin"></param>
        /// <returns></returns>
        public async Task<User> LoadByLoginAsync(string sLogin)
        {
            IQueryable<User> query = _context.Users.Where(c => c.UsrLogin == sLogin)
                .Include(r => r.UsrDefaultRoleNavigation);
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }


        /// <summary>
        /// Get All project managers - Role == 2
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> ListManagersAsync()
        {
            IQueryable<User> query = _context.Users
                            .Where(e => e.UsrActive.HasValue && e.UsrActive.Value == 1)
                            .Where(e => e.UsrDefaultRole == Constants.Roles.Supervisor)
                             .Include(x => x.UsrDefaultRoleNavigation)
                            .AsNoTracking();

            return await query.OrderBy(oManager => oManager.UsrLast).ThenBy(oManager => oManager.UsrFirst).ToListAsync();
        }


        /// <summary>
        /// Get Project manager record
        /// </summary>
        /// <param name="projectID">project id</param>
        /// <returns></returns>
        public async Task<User> LoadProjectManagerAsync(int projectID)
        {
            var puRecord =
                     from pu in _context.ProjectUsers
                     where pu.PuProjectId == projectID
                     select new
                     {
                         userID = pu.PuUserId,
                         roleID = pu.PuRoleId
                     };

            IQueryable<User> query = from u in _context.Users
                                     where puRecord.Any(pu => pu.userID == u.UsrId && pu.roleID == Constants.Roles.Supervisor)
                                     select u;

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        /// <summary>
        /// Employee Hour allocations
        /// </summary>
        /// <param name="periodID">Period ID</param>
        /// <param name="employeeID">Employee ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<HourAllocation>> ListAllocationsAsync(int periodID, int employeeID)
        {
            var query =
                from ha in _context.HourAllocations
                join pu in _context.ProjectUsers on ha.HapuId equals pu.PuId
                where pu.PuUserId == employeeID
                where ha.HaPeriodId == periodID
                select ha;

            return await query.Include(ha => ha.Hapu).AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Employee Hour Allocations - employee totals
        /// </summary>
        /// <param name="periodID">Period ID</param>
        /// <param name="employeeID">Employee ID</param>
        /// <returns></returns>

        public async Task<EmployeeTotals> GetEmployeeTotals(int periodID, int employeeID)
        {
            var employeeTotals = new EmployeeTotals();

            var lstParams = new[] {
                    new SqlParameter("@periodID", periodID ),
                    new SqlParameter("@employeeID", employeeID),
                };

            var oConnection = _context.Database.GetDbConnection();
            oConnection.Open();

            var oCommand = oConnection.CreateCommand();
            oCommand.Connection = oConnection;
            oCommand.CommandType = CommandType.StoredProcedure;
            oCommand.CommandText = "[dbo].usp_selEmployeeTotals";
            oCommand.Parameters.AddRange(lstParams.ToArray());

            using (var dbDataReader = await oCommand.ExecuteReaderAsync())
            {
                //read  db record
                while (dbDataReader.Read())
                {
                    employeeTotals = new EmployeeTotals(dbDataReader); //single record
                }

            }

            await oCommand.DisposeAsync();

            return employeeTotals;
        }


        /// <summary>
        /// List UnAllocated Projects
        /// </summary>
        /// <param name="periodID"></param>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProjectUser>> ListUnallocatedProjectsAsync(int periodID, int employeeID)
        {
            var lstPU = new List<ProjectUser>();

            var lstParams = new[] {
                    new SqlParameter("@periodID", periodID ),
                    new SqlParameter("@employeeID", employeeID),
                };

            var oConnection = _context.Database.GetDbConnection();

            var oCommand = oConnection.CreateCommand();
            oCommand.Connection = oConnection;
            oConnection.Open();

            oCommand.CommandType = CommandType.StoredProcedure;
            oCommand.CommandText = "[dbo].usp_selEmployeeUnallocatedProjects";
            oCommand.Parameters.AddRange(lstParams.ToArray());

            using (var dbDataReader = await oCommand.ExecuteReaderAsync())
            {
                //read  db record  - Project_User
                while (dbDataReader.Read())
                {
                    var puRecord = new ProjectUser()
                    {
                        PuId = (int)dbDataReader["puID"],
                        PuActive = (bool)dbDataReader["puActive"],
                        PuProjectId = (int)dbDataReader["puProjectID"],
                        PuUserId = (int)dbDataReader["puUserID"],
                        PuRoleId = (int)dbDataReader["puRoleID"],
                        PuCreatedBy = (string)dbDataReader["puCreatedBy"],
                        PuCreatedDate = (DateTime)dbDataReader["puCreatedDate"],
                        PuModifiedBy = (string)dbDataReader["puModifiedBy"],
                        PuModifiedDate = (DateTime)dbDataReader["puModifiedDate"]
                    };

                    lstPU.Add(puRecord);

                }

                oCommand.Dispose();
            }
            return lstPU;

        }


        /// <summary>
        /// Update user - FTE/Default role/Hours
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task UpdateUserAsync(UserModel model)
        {
            string sql = "EXEC usp_updUser @userID, @roleID, @FTE, @WillTrackHours, @loggedOnUser";

            var lstParams = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@userID", Value = model.ID },
                new SqlParameter { ParameterName = "@roleID", Value = model.RoleID },
                new SqlParameter { ParameterName = "@FTE", Value = model.FTE.HasValue ? model.FTE.Value : System.DBNull.Value },
                new SqlParameter { ParameterName = "@WillTrackHours", Value = model.WillTrackHours.HasValue ? model.WillTrackHours.Value : System.DBNull.Value },
                new SqlParameter { ParameterName = "@loggedOnUser", Value = _sLoginName }

            };

            await _context.Database.ExecuteSqlRawAsync(sql, lstParams.ToArray());
        }

        /// <summary>
        /// Login verification
        /// </summary>
        /// <param name="sLogin"></param>
        /// <returns></returns>
        public bool UserExists(string sLogin)
        {
            return _context.Users.Any(e => e.UsrLogin == sLogin);
        }

        /// <summary>
        /// Employees By Supervisor
        /// </summary>
        /// <param name="mgrID">Manager ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> ListByManagerAsync(int mgrID)
        {

            IQueryable<User> query =
            (
                        from u in _context.Users
                        join uSupv in _context.UserSupervisors on u.UsrId equals uSupv.UsUserId
                        join r in _context.Roles on u.UsrDefaultRole equals r.RId
                        where uSupv.UsUserSupervisorId == mgrID
                        where u.UsrActive.HasValue
                        where u.UsrActive.Value == 1
                        select u

            );


            return await query.OrderBy(u => u.UsrLast).ThenBy(u => u.UsrFirst).AsNoTracking().ToListAsync();


        }
        #endregion
    }
}
