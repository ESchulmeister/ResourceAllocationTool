using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Data.SqlClient;

namespace ResourceAllocationTool.Services
{
    public class RoleRepository : IRoleRepository
    {
        #region Variables
           private readonly TechCMSContext _context;
        private readonly IMemoryCache _cache;
        private string _sLoginName;
        #endregion

        #region constructors

        public RoleRepository(TechCMSContext context, IMemoryCache cache, IHttpContextAccessor oHttpContextAccessor)
        {
            _context = context;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _sLoginName = oHttpContextAccessor.GetIdentity();
        }


        #endregion

        #region Methods

        /// <summary>
        /// List active roles
        /// </summary>
        /// <returns>JSON Array -- Roles</returns>
        public async Task<IEnumerable<Role>> ListAsync()
        {
            //check cache
            if (_cache.TryGetValue(Constants.CacheKeys.Roles, out IEnumerable<Role> lstRoles))
            {
                return lstRoles;
            }

            IQueryable<Role> query = _context.Roles
                .Where(e => e.RActive == true)
                .AsNoTracking();

            lstRoles = await query.ToListAsync();

            if (lstRoles.Any())
            {
                var cacheExp = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(Constants.CacheExpHrs),
                    Priority = CacheItemPriority.Normal,
                };

                _cache.Set(Constants.CacheKeys.Roles, lstRoles, cacheExp);
            }


            return lstRoles.OrderBy(oRole => oRole.RName);
        }


        /// <summary>
        /// List non-supervisor roles
        /// </summary>
        /// <returns>JSON Array -- Role</returns>
        public async Task<IEnumerable<Role>> ListNonSupervisorsAsync()
        {

            IQueryable<Role> query = _context.Roles
                .Where(e => e.RActive == true)
                .Where(e => !e.RSupervisor)
                .AsNoTracking();

            var lstRoles = await query.ToListAsync();

            return lstRoles.OrderBy(oRole => oRole.RName);
        }



        /// <summary>
        /// Save Role - Name change
        /// </summary>
        /// <param name="model">JSON - RoleModel</param>
        public async Task SaveRoleAsync(RoleModel model)
        {

            string sql = "EXEC usp_execRole @roleID, @roleName, @loggedOnUser";

            var lstParams = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@roleID", Value = (model.ID == 0 ? System.DBNull.Value : model.ID) },
                new SqlParameter { ParameterName = "@roleName", Value = model.Name },
                new SqlParameter { ParameterName = "@loggedOnUser", Value = _sLoginName }

            };

            await _context.Database.ExecuteSqlRawAsync(sql, lstParams.ToArray());

            this.UpdateCache();

        }

        /// <summary>
        /// Delete Role
        /// </summary>
        /// <param name="roleID">Role ID - FromRoute</param>
        public async Task DeleteRoleAsync(int roleID)
        {

            string sql = "EXEC usp_delRole @roleID, @loggedInUser";

            var lstParams = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@roleID", Value = roleID },
                    new SqlParameter { ParameterName = "@loggedInUser", Value = _sLoginName }

                };

            await _context.Database.ExecuteSqlRawAsync(sql, lstParams.ToArray());

            this.UpdateCache();

        }

        /// <summary>
        /// Reset - update cache
        /// Used on each  action - update/delete
        /// </summary>
        private async void UpdateCache()
        {
            await _cache.Set(Constants.CacheKeys.Roles, this.ListAsync(), DateTime.Now.AddHours(Constants.CacheExpHrs));

        }
        #endregion
    }
}
