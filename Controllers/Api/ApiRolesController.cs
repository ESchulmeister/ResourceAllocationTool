using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using ResourceAllocationTool.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;

namespace ResourceAllocationTool.Controllers
{
    [Route("api/[controller]")]
    public class ApiRolesController : Controller
    {
        #region Variables
        private readonly IRoleRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        #endregion

        #region constructors
        public ApiRolesController(IRoleRepository repository, IMapper mapper, IMemoryCache memoryCache, ILogger<ApiRolesController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = memoryCache;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// List of  Roles
        /// </summary>
        /// <param name="all">Bool Flag - all/tracked Hrs</param>
        /// <param name="loadOptions">DevExpress</param>
        /// HTTP GET .../api/apiRoles
        [HttpGet]
        public async Task<IActionResult> List(bool all)
        {
            try
            {
                IEnumerable<Role> lstData = null;
                if (all)
                {
                    lstData = await _repository.ListAsync();
                }
                else   //select @ProjectTeams
                {
                    lstData = await _repository.ListNonSupervisorsAsync();
                }

                _mapper.Map<IEnumerable<Role>, IEnumerable<RoleModel>>(lstData);
                IEnumerable<RoleModel> lstRoles = _mapper.Map<IEnumerable<Role>, IEnumerable<RoleModel>>(lstData);
                return Json(await Task.Run(() => lstRoles));
            }
            catch (Exception ex)
            {

                _logger.LogError($"Liist Roles - All:{all}");
                var msg = ex.Message;
                _logger.LogError(msg);
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Add Role
        /// </summary>
        /// HTTP POST .../api/apiRoles
        /// <param name="values">JSON - ID/Name</param>
        [HttpPost]
        public async Task<IActionResult> AddRole(string values)
        {
            string sNewName = string.Empty;

            try
            {
                var oRole = new RoleModel();

                values.GetJsonValue<string>("Name", string.Empty, out sNewName);
                JsonConvert.PopulateObject(values, oRole);
                await _repository.SaveRoleAsync(oRole);

                return Ok(oRole);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Add Role - Name:{sNewName}");

                var msg = ex.Message;
                _logger.LogError(msg);
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Save Role
        /// </summary>
        /// HTTP PUT - update - .../api/apiRoles/
        /// <param name="key">Role ID</param>
        /// <param name="values">JSON body - Name</param>
        [HttpPut]
        public async Task<IActionResult> EditRole(int key, string values)
        {
            string sNewName = string.Empty;

            try
            {

                values.GetJsonValue<string>("Name", string.Empty, out sNewName);

                var model = new RoleModel()
                {
                    ID = key,
                    Name = sNewName
                };

                await _repository.SaveRoleAsync(model);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Edit Role - ID:{key}/Name:{sNewName}");

                var msg = ex.Message;
                _logger.LogError(msg);
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Delete Role
        /// </summary>
        /// HTTP DELETE .../api/apiRoles/{roleID}
        /// <param name="key">Role ID</param>
        [HttpDelete]
        public async Task<IActionResult> DeleteRole(int key)
        {

            try
            {
                await _repository.DeleteRoleAsync(key);
                return Ok();
            }
            catch (Exception ex)
            {

                _logger.LogError($"Delete Role -  ID:{key}");
                var msg = ex.Message;
                _logger.LogError(msg);
                return BadRequest(msg);
            }
        }

        #endregion
    }
}
