using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ResourceAllocationTool.Services
{
    public class ProjectRepository : IProjectRepository
    {
        #region Variables
        private readonly TechCMSContext _context;
        private string _sLoginName;
        private IMemoryCache _cache;
        #endregion

        #region constructors


        //TechCMSContext context,
        public ProjectRepository( IMemoryCache cache, IHttpContextAccessor oHttpContextAccessor)
        {
           // _context = context;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _sLoginName = oHttpContextAccessor.GetIdentity();
        }

        #endregion


        #region methods

        /// <summary>
        /// Get Active Projects
        /// </summary>
        /// <returns>JSON array - List of Projects</returns>
        public async Task<IEnumerable<Project>> ListAsync()
        {
            //check cache
            if (_cache.TryGetValue(Constants.CacheKeys.Projects, out IEnumerable<Project> lstProjects))
            {
                return lstProjects;
            }

            // IQueryable<Project> query = _context.Projects.Where(c => (!c.PrjActive.HasValue) || (c.PrjActive.Value == 1));

            IQueryable<Project> query = _context.Projects;

            //projects & project_user 
            lstProjects = await query.Include(p => p.ProjectUsers)
                .AsNoTracking()
                .OrderBy(oProject => oProject.PrjName).ToListAsync();

            if (lstProjects.Any())
            {
                var cacheExp = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(Constants.CacheExpHrs),
                    Priority = CacheItemPriority.Normal,
                };

                _cache.Set(Constants.CacheKeys.Projects, lstProjects, cacheExp);
            }


            return lstProjects;
        }

        /// <summary>
        /// List tracked - dbo.Projects.PRJ_Apps includes 16
        /// </summary>
        /// <returns>JSON array -- Projects</returns>
        public async Task<IEnumerable<Project>> ListTrackedAsync()
        {

            var lstProjects = await this.ListAsync();

            return lstProjects.Where(oProject => oProject.PrjApps.HasValue
                        && ((oProject.PrjApps.Value & Constants.ApplicationBit) == Constants.ApplicationBit));
        }

        /// <summary>
        /// Project By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>JSON -- Project</returns>
        public async Task<Project> GetByIDAsync(int id)
        {
            IQueryable<Project> query = _context.Projects.Where(c => c.PrjId == id);

            return await query.AsNoTracking().FirstOrDefaultAsync();

        }


        /// <summary>
        /// Update Project
        /// </summary>
        /// <param name="projectID">Project iD</param>
        /// <param name="mgrID">Manager ID</param>
        /// <param name="bWillTrackHours">Track Hours Flag</param>
        public async Task UpdateProjectAsync(int projectID, int mgrID, bool? bWillTrackHours)
        {
            string sql = "EXEC usp_execProject @projectID, @managerID, @WillTrackHours, @loggedOnUser";

            var lstParams = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@projectID", Value = projectID },
                new SqlParameter { ParameterName = "@managerID", Value = (mgrID == 0) ? System.DBNull.Value : mgrID },
                new SqlParameter { ParameterName = "@WillTrackHours", Value = bWillTrackHours.HasValue ? bWillTrackHours.Value : System.DBNull.Value },
                new SqlParameter { ParameterName = "@loggedOnUser", Value = _sLoginName },
            };

            await _context.Database.ExecuteSqlRawAsync(sql, lstParams.ToArray());

            this.UpdateCache();

        }

        /// <summary>
        /// Save project team - add/modify project_user record
        /// </summary>
        /// <param name="model">ProjectUserModel</param>
        public async Task SaveProjectTeamAsync(ProjectUserModel model)
        {
            string sql = "EXEC usp_execProjectTeam @puID, @projectID, @userID, @roleID, @loggedInUser";

            var lstParams = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@puID", Value = ( model.ID == 0 ) ? System.DBNull.Value : model.ID},
                    new SqlParameter { ParameterName = "@projectID", Value = ( model.ProjectID == 0 ) ? System.DBNull.Value : model.ProjectID},
                    new SqlParameter { ParameterName = "@userID", Value = ( model.UserID == 0 ) ? System.DBNull.Value : model.UserID},
                    new SqlParameter { ParameterName = "@roleID", Value = model.RoleID },
                    new SqlParameter { ParameterName = "@loggedInUser", Value = _sLoginName }
                };

            await _context.Database.ExecuteSqlRawAsync(sql, lstParams.ToArray());

        }


        /// <summary>
        /// Save Project Allocations
        /// </summary>
        /// <param name="projectUserID"></param>
        /// <param name="periodID"></param>
        /// <param name="estmatedHours"></param>
        /// <param name="actualHours"></param>
        public async Task SaveHourAllocationAsync(int projectUserID, int periodID, double? estmatedHours, double? actualHours)
        {
            string sql = "EXEC usp_updHourAllocation @projectUserID, @periodID, @estimatedHours, @actualHours, @loggedInUser";

            var lstParams = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@projectUserID", Value = projectUserID},
                    new SqlParameter { ParameterName = "@periodID", Value = periodID},
                    new SqlParameter { ParameterName = "@estimatedHours", Value = ( estmatedHours.HasValue ) ? estmatedHours.Value : System.DBNull.Value },
                    new SqlParameter { ParameterName = "@actualHours", Value = ( actualHours.HasValue ) ? actualHours.Value : System.DBNull.Value },
                    new SqlParameter { ParameterName = "@loggedInUser", Value =_sLoginName }
                };

            await _context.Database.ExecuteSqlRawAsync(sql, lstParams.ToArray());
        }


        /// <summary>
        /// Deactivate Project User
        /// </summary>
        /// <param name="id">Project User ID</param>
        public async Task DeleteProjectUserAsync(int id)
        {
            string sql = "EXEC usp_delProjectUser @puID, @loggedInUser";

            var lstParams = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@puID", Value = id },
                    new SqlParameter { ParameterName = "@loggedInUser", Value = _sLoginName }
                };

            await _context.Database.ExecuteSqlRawAsync(sql, lstParams.ToArray());
        }

        /// <summary>
        /// Project Hour Allocations, including project_user details
        /// </summary>
        /// <param name="projectID">Project ID</param>
        /// <param name="periodID">Period ID</param>
        /// <returns>JSON Array -- HourAllocation</returns>
        public async Task<IEnumerable<HourAllocation>> ListAllocationsAsync(int projectID, int periodID)
        {
            var query =
                from ha in _context.HourAllocations
                join pu in _context.ProjectUsers on ha.HapuId equals pu.PuId
                where pu.PuProjectId == projectID
                where ha.HaPeriodId == periodID
                select ha;

            return await query.Include(ha => ha.Hapu).AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Unallocated project_user records
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="periodID"></param>
        /// <returns>JSON Array -- ProjectUser</returns>
        public async Task<IEnumerable<ProjectUser>> ListUnallocatedAsync(int projectID, int periodID)
        {
            var query =
                from pu in _context.ProjectUsers
                where pu.PuProjectId == projectID
                where pu.PuActive == true
                where !pu.HourAllocations.Any(ha => ha.HapuId == pu.PuId && ha.HaPeriodId == periodID)
                select pu;

            return await query.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Project Hour Allocations - employee hours
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="periodID"></param>
        /// <returns>JSON Array -- UsedHours</returns>
        public async Task<IEnumerable<UsedHours>> ListHoursUsedAsync(int projectID, int periodID)
        {
            var lstUsedHours = new List<UsedHours>();

            var lstParams = new[] {
                new SqlParameter("@periodID", periodID ),
                new SqlParameter("@projectID", projectID),
            };

            var oConnection = _context.Database.GetDbConnection();
            oConnection.Open();

            var oCommand = oConnection.CreateCommand();
            oCommand.Connection = oConnection;
            oCommand.CommandType = CommandType.StoredProcedure;
            oCommand.CommandText = "[dbo].usp_selHoursUsed";
            oCommand.Parameters.AddRange(lstParams.ToArray());

            using (var dbDataReader = await oCommand.ExecuteReaderAsync())
            {
                //read  db record
                while (dbDataReader.Read())
                {
                    var oUsedHours = new UsedHours(dbDataReader);  //single record
                    lstUsedHours.Add(oUsedHours);
                }
            }

            await oCommand.DisposeAsync();

            return lstUsedHours;
        }


        /// <summary>
        /// Reset - update cache
        /// </summary>
        private async void UpdateCache()
        {
            await _cache.Set(Constants.CacheKeys.Projects, this.ListAsync(), DateTime.Now.AddHours(Constants.CacheExpHrs));

        }

        #endregion
    }
}
