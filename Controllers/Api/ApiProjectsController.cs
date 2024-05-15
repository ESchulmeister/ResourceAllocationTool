using AutoMapper;
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

namespace ResourceAllocationTool.Controllers
{
    [Route("api/[controller]")]
    public class ApiProjectsController : Controller
    {

        #region Variables
        private readonly IProjectRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        #endregion

        #region constructors

        public ApiProjectsController(IProjectRepository repository, IMapper mapper,
                            IMemoryCache memoryCache, ILogger<ApiProjectsController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = memoryCache;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// List projects
        /// </summary>
        /// HTTP GET ../api/apiProjects
        /// <param name="all">bool flag - all vs only tracked</param>
        /// <param name="loadOptions">DevExpress</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> List(bool all)
        {
        

            try
            {
                IEnumerable<Project> lstData = null;
                if (all)
                {
                    lstData = await _repository.ListAsync();
                }
                else
                {
                    lstData = await _repository.ListTrackedAsync();
                }

                _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectModel>>(lstData);
                IEnumerable<ProjectModel> lstProjects = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectModel>>(lstData);
                return Json(await Task.Run(() => lstProjects));

            }
            catch (Exception ex)
            {
                _logger.LogError($"Projects.List - Track Hours?:{all}");
                var errMsg = ex.Message;
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }

        }

        /// <summary>
        /// Assign Project Manager
        /// </summary>
        /// HTTP PUT ../api/apiProjects
        /// <param name="key">Projectid</param>
        /// <param name="values">json - managerID/Track Hours Flag</param>
        [HttpPut]
        public async Task<IActionResult> UpdateProject(int key, string values)
        {
            int managerID;
            string log = $"Update Project - ProjectID:{key}";

            try
            {

                int projectID = key;

                bool bChangeManager = values.GetJsonValue<int>("ManagerID", 0, out managerID);
                if (!bChangeManager)
                {
                    managerID = 0;
                }


                bool bWillTrackHours;
                bool bChangeTrackHours = values.GetJsonValue<bool>("WillTrackHours", false, out bWillTrackHours);

                await _repository.UpdateProjectAsync(projectID, managerID, bChangeTrackHours ? bWillTrackHours : null);


                if (bChangeManager)
                {
                    _logger.LogInformation($"{log}/manager id:{managerID}");
                }

                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError(log);
                var errMsg = ex.Message;
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }
        }

        #endregion
    }
}

namespace ResourceAllocationTool
{
    class DataSourceLoadOptions
    {
    }
}