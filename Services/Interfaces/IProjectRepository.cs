using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResourceAllocationTool.Services
{
    /// <summary>
    /// apiProjects endpoints
    /// </summary>
    public interface IProjectRepository
    {

        Task<IEnumerable<Project>> ListAsync();
        Task<IEnumerable<Project>> ListTrackedAsync();

        Task<Project> GetByIDAsync(int key);

        Task UpdateProjectAsync(int projID, int mgrID, bool? bWillTrackHours);

        Task SaveProjectTeamAsync(ProjectUserModel model);

        Task SaveHourAllocationAsync(int projectUserID, int periodID, double? estmatedHours, double? actualHours);

        Task DeleteProjectUserAsync(int id);

        Task<IEnumerable<HourAllocation>> ListAllocationsAsync(int projectID, int periodID);

        Task<IEnumerable<UsedHours>> ListHoursUsedAsync(int projectID, int periodID);

        Task<IEnumerable<ProjectUser>> ListUnallocatedAsync(int projectID, int periodID);
    }
}
