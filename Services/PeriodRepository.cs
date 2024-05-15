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
    public class PeriodRepository : IPeriodRepository
    {
        #region Variables
        private readonly TechCMSContext _context;
        private string _sLoginName;
        private IMemoryCache _cache;
        #endregion

        #region constructors


       //TechCMSContext context,
        public PeriodRepository( IMemoryCache cache, IHttpContextAccessor oHttpContextAccessor)
        {
           // _context = context;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _sLoginName = oHttpContextAccessor.GetIdentity();
        }

        #endregion


        /// <summary>
        /// List active periods
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Period>> ListAsync()
        {
            if (_cache.TryGetValue(Constants.CacheKeys.Periods, out IEnumerable<Period> lstPeriods))
            {
                return lstPeriods;
            }

            IQueryable<Period> query = _context.Periods
                .Where(e => e.PerActive == true)
                .AsNoTracking();

            //projects & project_user 
            lstPeriods = await query.ToListAsync();

            if (lstPeriods.Any())
            {
                var cacheExp = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(Constants.CacheExpHrs),
                    Priority = CacheItemPriority.Normal,
                };

                _cache.Set(Constants.CacheKeys.Periods, lstPeriods, cacheExp);
            }


            return lstPeriods;
        }

        /// <summary>
        /// Save Period 
        /// </summary>
        /// <param name="model">Json</param>
        public async Task SavePeriodAsync(PeriodModel model)
        {
            string sql = "EXEC usp_execPeriod @periodID, @periodName, @workHours, @workDays, @loggedOnUser";

            var lstParams = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@periodID", Value = (model.ID == 0 ? System.DBNull.Value : model.ID) },
                new SqlParameter { ParameterName = "@periodName", Value = model.Name },
                new SqlParameter { ParameterName = "@workHours", Value = model.WorkHours },
                new SqlParameter { ParameterName = "@workDays", Value = model.WorkDays },
                new SqlParameter { ParameterName = "@loggedOnUser", Value = _sLoginName }
            };

            await _context.Database.ExecuteSqlRawAsync(sql, lstParams.ToArray());

            this.UpdateCache();

        }

        /// <summary>
        /// Delete Period
        /// </summary>
        /// <param name="periodID">Key == ID</param>
        public async Task DeletePeriodAsync(int periodID)
        {

            string sql = "EXEC usp_delPeriod @periodID, @loggedInUser";

            var lstParams = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@periodID", Value = @periodID },
                    new SqlParameter { ParameterName = "@loggedInUser", Value = _sLoginName }
                };

            await _context.Database.ExecuteSqlRawAsync(sql, lstParams.ToArray());

            this.UpdateCache();

        }


        /// <summary>
        /// Get Period by ID - Instance
        /// </summary>
        /// <param name="id"></param>
        /// <returns>JSON -- Period</returns>
        public async Task<Period> GetByIDAsync(int id)
        {
            IQueryable<Period> query = _context.Periods.Where(c => c.PerId == id);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }


        /// <summary>
        /// Reset - update cache
        /// </summary>
        private async void UpdateCache()
        {
            await _cache.Set(Constants.CacheKeys.Periods, this.ListAsync(), DateTime.Now.AddHours(Constants.CacheExpHrs));

        }
    }
}
