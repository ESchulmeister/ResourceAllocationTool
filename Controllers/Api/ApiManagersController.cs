using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using ResourceAllocationTool.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResourceAllocationTool.Controllers
{
    [Route("api/[controller]")]
    public class ApiManagersController : Controller
    {

        #region Variables
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        #endregion

        #region constructors

        public ApiManagersController(IUserRepository repository, IMapper mapper, ILogger<ApiManagersController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load manager record
        /// </summary>
        /// HTTP GET : ..api/ApiManagers/{projectID}
        /// <param name="projectID">Project ID</param>
        /// <returns>JSON - UserModel</returns>
        [HttpGet("{projectID:int}")]
        public async Task<IActionResult> Load(int projectID)
        {
            try
            {
                var result = await _repository.LoadProjectManagerAsync(projectID);
                var oUserModel = _mapper.Map<UserModel>(result);
                return Ok(oUserModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Load Project Manager  - ProjectID:{projectID}");

                var errMsg = ex.Message;
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }
        }

        /// <summary>
        /// List Managers
        /// </summary>
        /// <param name="loadOptions">DevExpress</param>
        /// <returns>JSON array - UserModel</returns>
        public async Task<IActionResult> List()
        {
            try
            {

                var lstData = await _repository.ListManagersAsync();
                IEnumerable<UserModel> lstManagers = _mapper.Map<IEnumerable<User>, IEnumerable<UserModel>>(lstData);
                return Json(await Task.Run(() => lstManagers));
            }
            catch (Exception ex)
            {
                _logger.LogError($"List Managers");
                var errMsg = ex.Message;
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }
        }
        #endregion
    }
}
