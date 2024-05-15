using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
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
    public class ApiEmployeeAllocationsController : Controller
    {
        #region Variables
        private readonly IProjectRepository _projRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        #endregion

        #region constructors

        public ApiEmployeeAllocationsController(IProjectRepository projRepository, IUserRepository userRepository,
                                            IRoleRepository roleRepository, IMapper mapper, ILogger<ApiEmployeeAllocationsController> logger)
        {
            _projRepository = projRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Allocations By Period & Employee
        /// </summary>
        /// HTTP GET : ../api/ApiEmployeeAllocations/{employeeID}?periodID={periodID} 
        /// <param name="periodID"></param>
        /// <param name="employeeID"></param>
        /// <param name="loadOptions">DevExpress</param>
        /// <returns>JSON Array--- list -- HourAllocationModel</returns>
        [HttpGet]
        public async Task<ActionResult<HourAllocationModel>> ListByPeriodAndEmployee(int periodID, int employeeID)
        {

            try
            {
                if (employeeID == 0 || periodID == 0)
                {
                    return Json(new List<HourAllocationModel>());
                }

                var lstData = await _userRepository.ListAllocationsAsync(periodID, employeeID);
                List<HourAllocationModel> lstHourAllocations = _mapper.Map<IEnumerable<HourAllocation>, List<HourAllocationModel>>(lstData);

                var lstUnallocated = await _userRepository.ListUnallocatedProjectsAsync(periodID, employeeID);

                //unallocated list 
                lstUnallocated.ToList().ForEach(oProjectUser => this.AddUnallocated(lstHourAllocations, oProjectUser, periodID));

                //roles
                var lstRoles = await _roleRepository.ListAsync();
                IEnumerable<RoleModel> lstRoleModels = _mapper.Map<IEnumerable<Role>, IEnumerable<RoleModel>>(lstRoles);
                this.MapRoleData(lstHourAllocations, lstRoleModels);

                //projects
                var lstProjects = await _projRepository.ListAsync();
                IEnumerable<ProjectModel> lstProjectModels = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectModel>>(lstProjects);
                this.MapProjectData(lstHourAllocations, lstProjectModels);

                return Json(await Task.Run(() =>lstHourAllocations));
            }
            catch (Exception ex)
            {
                _logger.LogError($"ListByPeriodAndEmployee - EmployeeID:{employeeID}/PeriodID:{periodID}");

                var msg = ex.Message;
                _logger.LogError(msg);
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Add Unallocated HourAllocation record
        /// </summary>
        /// <param name="lstHourAllocations"></param>
        /// <param name="oProjectUser"></param>
        /// <param name="periodID"></param>
        private void AddUnallocated(List<HourAllocationModel> lstHourAllocations, ProjectUser oProjectUser, int periodID)
        {
            lstHourAllocations.Add(new HourAllocationModel()
            {
                PeriodID = periodID,
                ProjectUserID = oProjectUser.PuId,
                ProjectUser = new ProjectUserModel()
                {
                    RoleID = oProjectUser.PuRoleId,
                    ProjectID = oProjectUser.PuProjectId
                }
            });
        }

        /// <summary>
        /// Employee totals
        /// </summary>
        /// HTTP GET : .../api//ApiEmployeeAllocations/{employeeID}
        /// <param name="employeeID">Employee -  selEmployees</param>
        /// <param name="periodID">Period - selPeriods</param>
        /// <returns>json - single totals record</returns>
        [HttpGet("{employeeID:int}")]
        public async Task<ActionResult<EmployeeTotalsModel>> LoadEmployeeTotals(int employeeID, int periodID)
        {

            try
            {

                if (employeeID == 0 || periodID == 0)
                {
                    return this.BadRequest("Required information not detected");
                }

                var empTotals = await _userRepository.GetEmployeeTotals(periodID, employeeID);

                //data classes model
                EmployeeTotalsModel oEmployeeTotalsModel = _mapper.Map<EmployeeTotals, EmployeeTotalsModel>(empTotals);
                return Ok(oEmployeeTotalsModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"LoadEmployeeTotals - EmployeeID:{employeeID}/PeriodID:{periodID}");

                var msg = ex.Message;
                _logger.LogError(msg);
                return BadRequest(msg);
            }

        }

        /// <summary>
        /// Update project user allocation record
        /// </summary>
        /// <param name="key">projectUserID|PeriodID</param>
        /// HTTP PUT ../api/apiEmployeeAllocations
        /// <param name="values">Json - Estimated/Actual hours</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> SaveHourAssociation(string key, string values)
        {
            try
            {

                bool bOk = await _projRepository.SaveHourAssociation(key, values);

                if (!bOk)
                {
                    return this.BadRequest("Key is missing or invalid");
                }

                return this.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"EmployeeAllocations.SaveHourAllocation -  Key:{key}");

                var msg = ex.Message;

                _logger.LogError(msg);
                return BadRequest(msg);
            }
        }

        /// <summary>
        /// Map role  - set project role
        /// </summary>
        /// <param name="lstHourAllocations"></param>
        /// <param name="lstRoleModels"></param>        
        private void MapRoleData(IEnumerable<HourAllocationModel> lstHourAllocations, IEnumerable<RoleModel> lstRoleModels)
        {
            foreach (var oProjectAllocationModel in lstHourAllocations)
            {
                var oRoleModel = lstRoleModels.FirstOrDefault(r => r.ID == oProjectAllocationModel.ProjectUser.RoleID);
                if (oRoleModel != null)
                {
                    oProjectAllocationModel.ProjectRole = oRoleModel.Name;
                }
            }
        }

        /// <summary>
        /// Map project  - set project name
        /// </summary>
        /// <param name="lstHourAllocations"></param>
        /// <param name="lstProjectModels"></param>
        private void MapProjectData(IEnumerable<HourAllocationModel> lstHourAllocations, IEnumerable<ProjectModel> lstProjectModels)
        {
            foreach (var oHourAllocationModel in lstHourAllocations)
            {
                var oProjectModel = lstProjectModels.FirstOrDefault(r => r.ID == oHourAllocationModel.ProjectUser.ProjectID);
                if (oProjectModel != null)
                {
                    oHourAllocationModel.ProjectName = oProjectModel.Name;
                }
            }
        }




        #endregion
    }
}
