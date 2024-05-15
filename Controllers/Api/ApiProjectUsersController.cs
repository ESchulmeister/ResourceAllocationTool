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
    public class ApiProjectUsersController : Controller
    {

        #region Variables
        private readonly IProjectRepository _projRepository;
        private readonly IUserRepository _usrRepository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        #endregion

        #region constructors

        public ApiProjectUsersController(IUserRepository usrRepository, IProjectRepository projRepository, IMapper mapper, ILogger<ApiProjectUsersController> logger)
        {
            _usrRepository = usrRepository;
            _projRepository = projRepository;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// List <ProjectUser> by projectID
        /// </summary>
        /// HTTP GET ../api/apiProjectUsers
        /// <param name="projectID">project id</param>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ProjectUserModel>> ListByProject(int projectID)
        {
            try
            {
                if (projectID == 0)
                {
                    return Json(new List<ProjectUserModel>());
                }

                var lstData = await _usrRepository.ListByProjectAsync(projectID);
                IEnumerable<ProjectUserModel> lstProjectUsers = _mapper.Map<IEnumerable<ProjectUser>, IEnumerable<ProjectUserModel>>(lstData);
                return Json(await Task.Run(() =>lstProjectUsers));
            }

            catch (Exception ex)
            {

                _logger.LogError($"ProjectUser.List By Project -  Project ID:{projectID}");
                var errMsg = ex.Message;
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }

        }



        /// <summary>
        /// Add employee to project
        /// </summary>
        /// HTTP POST ../api/apiProjectUsers
        /// <param name="values">json - userid/roleid/projectid</param>
        [HttpPost]
        public async Task<IActionResult> AddProjectUser(string values)
        {
            int userID = 0, roleID = 0, projectID = 0;

            try
            {

                values.GetJsonValue<int>("UserID", 0, out userID);
                values.GetJsonValue<int>("RoleID", 0, out roleID);
                values.GetJsonValue<int>("ProjectID", 0, out projectID);

                var model = new ProjectUserModel()
                {
                    ID = 0,
                    ProjectID = projectID,
                    UserID = userID,
                    RoleID = roleID
                };

                await _projRepository.SaveProjectTeamAsync(model);
                return Ok();

            }
            catch (Exception ex)
            {

                _logger.LogError($"AddProjectUser -  project id:{projectID}/" +
                                            $"user id:{userID}/" +
                                            $"role id:{roleID}");


                var errMsg = ex.Message;
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }

        }

        /// <summary>
        /// Update project-user - emp role update
        /// </summary>
        /// HTTP POST ../api/apiProjectUsers
        /// <param name="key">ID - projectUserID</param>
        /// <param name="values">json - userID/roleID/projectID</param>
        [HttpPut]
        public async Task<IActionResult> UpdateProjectUser(int key, string values)
        {
            int roleID = 0;

            try
            {

                values.GetJsonValue("RoleID", 0, out roleID);

                var model = new ProjectUserModel()
                {
                    ID = key,
                    RoleID = roleID
                };

                await _projRepository.SaveProjectTeamAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateProjectUser -  project id:{key}/" +
                                            $"role id:{roleID}");


                var errMsg = ex.Message;
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }
        }


        /// <summary>
        /// Deactivate project user
        /// </summary>
        /// HTTP DELETE ../api/apiProjectUsers/{projectuserid}
        [HttpDelete]
        public async Task<IActionResult> DeleteProjectUser(int key)
        {

            try
            {
                await _projRepository.DeleteProjectUserAsync(key);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete Project User -  project id:{key}");
                var errMsg = ex.Message;
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }
        }


        #endregion
    }
}
