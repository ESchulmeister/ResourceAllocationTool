using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using ResourceAllocationTool.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ResourceAllocationTool.Controllers
{
    [Route("api/[controller]")]
    public class ApiPeriodsController : Controller
    {

        #region Variables
        private readonly IPeriodRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        #endregion


        #region constructors

        public ApiPeriodsController(IPeriodRepository repository, IMapper mapper, IMemoryCache memoryCache,
            ILogger<ApiPeriodsController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = memoryCache;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// List Periods
        /// </summary>
        /// HTTP GET : ../api/apiPeriods
        /// <param name="loadOptions">DevExpress</param>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {

                var lstData = await _repository.ListAsync();

                _mapper.Map<IEnumerable<Period>, IEnumerable<PeriodModel>>(lstData);

                IEnumerable<PeriodModel> lstPeriods = _mapper.Map<IEnumerable<Period>, IEnumerable<PeriodModel>>(lstData);

                if (lstPeriods.Any())
                {

                    var cacheExp = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddHours(Constants.CacheExpHrs),
                        Priority = CacheItemPriority.Normal,
                    };

                    _cache.Set(Constants.CacheKeys.Periods, lstPeriods, cacheExp);
                }

                return Json(await Task.Run(() => lstPeriods));



            }
            catch (Exception ex)
            {
                _logger.LogError("Periods.List");

                var errMsg = ex.Message;
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }

        }


        /// <summary>
        /// Add Period
        /// </summary>
        /// HTTP PUT .../api/apiPeriods
        /// <param name="values">Json - id/name/hours/days</param>
        [HttpPost]
        public async Task<IActionResult> AddPeriod(string values)
        {

            string sName = string.Empty;
            float fWorkDays = 0, fWorkHours = 0;

            try
            {
                values.GetJsonValue<string>("Name", string.Empty, out sName);
                values.GetJsonValue<float>("WorkHours", 0, out fWorkHours);
                values.GetJsonValue<float>("WorkDays", 0, out fWorkDays);

                var model = new PeriodModel()
                {
                    ID = 0,
                    Name = sName,
                    WorkHours = fWorkHours,
                    WorkDays = fWorkDays
                };

                await _repository.SavePeriodAsync(model);

                return Ok();

            }
            catch (Exception ex)
            {

                _logger.LogError($"AddPeriod - " +
                             $"Name:{sName}" +
                             $" /WorkHours:{fWorkHours}" +
                             $"/WorDays:{fWorkDays}");

                var errMsg = ex.Message;

                _logger.LogError(errMsg);

                return BadRequest(errMsg);
            }

        }

        /// <summary>
        /// Modify period
        /// </summary>
        /// HTTP PUT ../api/apiPeriods
        /// <param name="key">period id</param>
        /// <param name="values">Json - id/name/hours/days</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> EditPeriod(int key, string values)
        {

            try
            {
                string sNewName;
                float fWorkDays = 0, fWorkHours = 0;

                //identify period
                var oDbPeriod = await _repository.GetByIDAsync(key);

                if (oDbPeriod == null)
                {
                    return BadRequest("Period Record Not Found");
                }

                JsonConvert.PopulateObject(values, oDbPeriod);

                values.GetJsonValue<string>("Name", string.Empty, out sNewName);

                values.GetJsonValue<float>("WorkHours", 0, out fWorkHours);

                values.GetJsonValue<float>("WorkDays", 0, out fWorkDays);

                var model = new PeriodModel()
                {
                    ID = key,
                    Name = string.IsNullOrWhiteSpace(sNewName) ? oDbPeriod.PerName : sNewName,
                    WorkHours = (fWorkHours == 0) ? oDbPeriod.PerWorkHours : fWorkHours,
                    WorkDays = (fWorkDays == 0) ? oDbPeriod.PerWorkDays : fWorkDays,
                };

                await _repository.SavePeriodAsync(model);

                return Ok();
            }
            catch (Exception ex)
            {

                _logger.LogError($"EditPeriod -  id: {key}");

                var errMsg = ex.Message;

                _logger.LogError(errMsg);

                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// Delete Period
        /// </summary>
        /// HTTP DELETE ..api/api/Periods/{periodID}
        /// <param name="key">Period id</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeletePeriod(int key)
        {

            try
            {
                await _repository.DeletePeriodAsync(key);


                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeletePeriod - id:{key}");

                var errMsg = ex.Message;

                _logger.LogError(errMsg);

                return BadRequest(errMsg);
            }
        }

        #endregion
    }
}
