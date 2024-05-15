using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using ResourceAllocationTool.Services;
using System;

using AutoMapper;

using System.Collections.Generic;
using System.Threading.Tasks;


namespace ResourceAllocationTool.Controllers
{
    [Route("api/[controller]")]
    public class ApiUsersController : Controller
    {

        #region Variables
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        #endregion

        #region constructors

        public ApiUsersController(IUserRepository repository, IMapper mapper, ILogger<ApiUsersController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Load User By login - Profile
        /// </summary>
        /// <param name="login">User Name</param>
        /// HTTP GET : https:../api/ApiUsers/[login]
        /// <returns>JSON  -- UserModel</returns>
        [HttpGet("{login:regex(^[[a-zA-Z]])}")]
        public async Task<ActionResult<UserModel>> LoadByLogin([FromRoute] string login)
        {
            try

            {

                if (!_repository.UserExists(login))
                {
                    var msg = $"Invalid login - {login}";
                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                var result = await _repository.LoadByLoginAsync(login);
                var oUserModel = _mapper.Map<UserModel>(result);
                return this.Ok(oUserModel);

            }
            catch (Exception ex)
            {
                _logger.LogError($"LoadByLogin - Login:{login}");
                var msg = ex.Message;
                _logger.LogError(msg);
                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Employees View
        /// </summary>
        /// <param value="all">Track Hours flag - all or only tracked user records</param>
        /// <param name="mgrID">Manager ID - EmployeesByManager</param>
        /// HTTP GET : ../api/ApiUsers
        /// <param name="loadOptions">DevExpress</param>
        /// <returns>JSON Array -- UserModel</returns>
        [HttpGet]
        public async Task<ActionResult<UserModel>> ListAll(bool all, int mgrID)
        {
            try
            {

                if (mgrID != 0)
                {
                    return await this.ListByManager(mgrID);
                }

                IEnumerable<User> lstData = null;

                if (all)
                {
                    lstData = await _repository.ListAllAsync();
                }
                else
                {
                    lstData = await _repository.ListTrackedAsync();
                }

                IEnumerable<UserModel> lstUsers = _mapper.Map<IEnumerable<User>, IEnumerable<UserModel>>(lstData);

                return Json(await Task.Run(() => lstUsers));
            }
            catch (Exception ex)
            {
                _logger.LogError($"ListUsers - Track Hours?:{all}");
                var msg = ex.Message;
                _logger.LogError(msg);
                return BadRequest(msg);
            }
        }

        /// <summary>
        /// List Available users by project
        /// </summary>
        /// HTTP GET : ../api/ApiUsers/{projectID}
        /// <param name="projectID"></param>
        /// <param name="loadOptions">DevExpress param</param>
        /// <returns>JSON array  -- UserModel</returns>
        [HttpGet("{projectID:int}")]
        public async Task<ActionResult<UserModel>> ListAvailable(int projectID)
        {

            try
            {

                var lstData = await _repository.ListAvailableAsync(projectID);
                IEnumerable<UserModel> lstUsers = _mapper.Map<IEnumerable<User>, IEnumerable<UserModel>>(lstData);
                return Json(await Task.Run(() => lstUsers));
            }
            catch (Exception ex)
            {

                _logger.LogError($"List Available Users -  Project ID:{projectID}");
                var msg = ex.Message;
                _logger.LogError(msg);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// HTTP PUT 
        /// </summary>
        /// <param name="key">employee id</param>
        /// <param name="values">Json -- FTE/RoleID/WillTrackHours</param>
        [HttpPut]
        public async Task<IActionResult> UpdateUser(int key, string values)
        {
            try
            {
                int roleID = 0;
                values.GetJsonValue<int>("RoleID", 0, out roleID);

                float dFTE = 0;
                bool bChangeFTE = values.GetJsonValue<float>("FTE", 0, out dFTE);

                bool bWillTrackHours = false;
                bool bChangeTrackHours = values.GetJsonValue<bool>("WillTrackHours", false, out bWillTrackHours);

                var model = new UserModel()
                {
                    ID = key,
                    RoleID = roleID,
                    FTE = bChangeFTE ? dFTE : null,
                    WillTrackHours = bChangeTrackHours ? bWillTrackHours : null
                };

                await _repository.UpdateUserAsync(model);
                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateUser - ID:{key}");
                var erMsg = ex.Message;
                _logger.LogError(erMsg);
                return BadRequest(erMsg);
            }
        }

        /// <summary>
        /// List Employees By Manager
        /// </summary>
        /// <param name="mgrID">Manager ID</param>
        /// <param name="loadOptions">DevExpress</param>
        /// <returns>JSON array -- UserModel</returns>
        protected async Task<ActionResult<UserModel>> ListByManager(int mgrID)
        {
            try
            {
                if (mgrID == 0)
                {
                    return Json(new List<UserModel>());
                }

                var lstData = await _repository.ListByManagerAsync(mgrID);
                IEnumerable<UserModel> lstUsers = _mapper.Map<IEnumerable<User>, IEnumerable<UserModel>>(lstData);
                return Json(await Task.Run(() =>lstUsers));
            }
            catch (Exception ex)
            {
                _logger.LogError($"List subordinates by manager - mgrID: {mgrID}");
                var msg = ex.Message;
                _logger.LogError(msg);
                return BadRequest(msg);
            }
        }
        #endregion
    }

}

