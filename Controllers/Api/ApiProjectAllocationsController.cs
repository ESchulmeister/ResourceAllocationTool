using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using ResourceAllocationTool.Services;
using System;
using AutoMapper;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceAllocationTool.Controllers
{
    [Route("api/[controller]")]
    public class ApiProjectAllocationsController : Controller
    {
        #region Variables
        private readonly IProjectRepository _projRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPeriodRepository _periodRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        #endregion

        #region constructors

        public ApiProjectAllocationsController(IProjectRepository projRepository, IUserRepository userRepository, IPeriodRepository periodRepository,
                                                                IRoleRepository roleRepository, IMapper mapper, ILogger<ApiProjectAllocationsController> logger)
        {
            _projRepository = projRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _periodRepository = periodRepository;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// List Project Hour Allocations
        /// </summary>
        /// HTTP GET ../api/apiProjectAllocations
        /// <param name="projectID">project id</param>
        /// <param name="periodID">period id</param>
        /// <param name="loadOptions">DevExpress</param>
        /// <returns>JSON array - ProjectAllocationModel</returns>        
        [HttpGet]
        public async Task<ActionResult<ProjectAllocationModel>> List(int projectID, int periodID)
        {

            try
            {
                if (projectID == 0 || periodID == 0)
                {
                    return Json(new List<ProjectAllocationModel>());
                }

                var lstData = await _projRepository.ListAllocationsAsync(projectID, periodID);
                List<ProjectAllocationModel> lstProjectAllocations = _mapper.Map<IEnumerable<HourAllocation>, List<ProjectAllocationModel>>(lstData);

                var lstUnallocated = await _projRepository.ListUnallocatedAsync(projectID, periodID);
                lstUnallocated.ToList().ForEach(u => this.AddToList(lstProjectAllocations, u, periodID));

                var lstUsers = await _userRepository.ListAllAsync();
                IEnumerable<UserModel> lstUserModels = _mapper.Map<IEnumerable<User>, IEnumerable<UserModel>>(lstUsers);

                var lstRoles = await _roleRepository.ListAsync();
                IEnumerable<RoleModel> lstRoleModels = _mapper.Map<IEnumerable<Role>, IEnumerable<RoleModel>>(lstRoles);

                var lstPeriods = await _periodRepository.ListAsync();
                IEnumerable<PeriodModel> lstPeriodModels = _mapper.Map<IEnumerable<Period>, IEnumerable<PeriodModel>>(lstPeriods);

                var lstHoursUsed = await _projRepository.ListHoursUsedAsync(projectID, periodID);

                this.MapUserData(lstProjectAllocations, lstUserModels);
                this.MapRoleData(lstProjectAllocations, lstRoleModels);
                this.MapPeriodData(lstProjectAllocations, lstPeriodModels);
                this.MapHoursUsed(lstProjectAllocations, lstHoursUsed);

                return Json(await Task.Run(() =>lstProjectAllocations));   //DevExtreme : DatraSourceLoaderOptions
            }
            catch (Exception ex)
            {
                _logger.LogError($"ProjectAllocation.List - projectID :{projectID}/periodID :{periodID}");
                var msg = ex.Message;
                _logger.LogError(msg);
                return BadRequest(msg);
            }

        }


        /// <summary>
        /// Update Project user allocation
        /// </summary>
        /// <param name="key">ProjectUserID|PeriodID</param>
        /// <param name="values"></param>
        [HttpPut]
        public async Task<IActionResult> SaveHourAssociation(string key, string values)
        {
            try
            {
                bool bOk = await _projRepository.SaveHourAssociation(key, values);

                if (!bOk)
                {
                    return this.BadRequest("Project Hour Allocation Key is missing/invalid");
                }

                return this.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Save Project Hour Allocation -  Key:{key}/Values:{values}");

                // cannot use ServerError status code due to DevExpress constraints
                var errMsg = ex.Message;
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }
        }


        /// <summary>
        /// Map User Data - Name/FTE
        /// </summary>
        /// <param name="lstProjectAllocations"></param>
        /// <param name="lstUserModels"></param>
        private void MapUserData(IEnumerable<ProjectAllocationModel> lstProjectAllocations, IEnumerable<UserModel> lstUserModels)
        {
            foreach (var oProjectAllocationModel in lstProjectAllocations)
            {
                var oUserModel = lstUserModels.FirstOrDefault(u => u.ID == oProjectAllocationModel.UserID);
                if (oUserModel != null)
                {
                    oProjectAllocationModel.UserFullNameReversed = oUserModel.FullNameReversed;
                    oProjectAllocationModel.FTE = oUserModel.FTE;
                }
            }
        }

        /// <summary>
        /// Map Role Data - Project Role -name
        /// </summary>
        /// <param name="lstProjectAllocations"></param>
        /// <param name="lstRoleModels"></param>
        private void MapRoleData(IEnumerable<ProjectAllocationModel> lstProjectAllocations, IEnumerable<RoleModel> lstRoleModels)
        {
            foreach (var oProjectAllocationModel in lstProjectAllocations)
            {
                var oRoleModel = lstRoleModels.FirstOrDefault(r => r.ID == oProjectAllocationModel.ProjectUser.RoleID);
                if (oRoleModel != null)
                {
                    oProjectAllocationModel.ProjectRole = oRoleModel.Name;
                }
            }
        }

        /// <summary>
        /// Map Period Data
        /// </summary>
        /// <param name="lstProjectAllocations"></param>
        /// <param name="lstPeriodModels"></param>
        private void MapPeriodData(IEnumerable<ProjectAllocationModel> lstProjectAllocations, IEnumerable<PeriodModel> lstPeriodModels)
        {
            foreach (var oProjectAllocationModel in lstProjectAllocations)
            {
                var oPeriodModel = lstPeriodModels.FirstOrDefault(r => r.ID == oProjectAllocationModel.PeriodID);
                if (oPeriodModel != null)
                {
                    oProjectAllocationModel.TotalHours = (float)(double)(oPeriodModel.WorkHours * oProjectAllocationModel.FTE);
                }
            }
        }

        private void MapHoursUsed(IEnumerable<ProjectAllocationModel> lstProjectAllocations, IEnumerable<UsedHours> lstHoursUsed)
        {
            foreach (var oProjectAllocationModel in lstProjectAllocations)
            {
                var oUsedHours = lstHoursUsed.FirstOrDefault(h => h.UserID == oProjectAllocationModel.UserID);
                if (oUsedHours != null)
                {
                    oProjectAllocationModel.UsedHours = oUsedHours.Hours;
                }
            }
        }

        private void AddToList(List<ProjectAllocationModel> lstProjectAllocations, ProjectUser oProjectUser, int iPeriodID)
        {
            var oProjectAllocationModel = new ProjectAllocationModel()
            {
                UserID = oProjectUser.PuUserId,
                PeriodID = iPeriodID,
                ProjectUserID = oProjectUser.PuId,
                ProjectUser = new ProjectUserModel()
                {
                    ID = oProjectUser.PuId,
                    UserID = oProjectUser.PuUserId,
                    RoleID = oProjectUser.PuRoleId,
                    ProjectID = oProjectUser.PuProjectId
                }
            };

            lstProjectAllocations.Add(oProjectAllocationModel);
        }
        #endregion
    }
}
