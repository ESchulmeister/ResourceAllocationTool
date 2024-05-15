
using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResourceAllocationTool.Services
{

    /// <summary>
    /// apiUsers endpoints
    /// </summary>
    public interface IUserRepository
    {
        Task<IEnumerable<User>> ListAllAsync();
        Task<IEnumerable<User>> ListTrackedAsync();

        Task<IEnumerable<User>> ListByManagerAsync(int mgrID);

        Task<IEnumerable<User>> ListAvailableAsync(int projectID);

        Task<EmployeeTotals> GetEmployeeTotals(int periodID, int employeeID);

        Task<IEnumerable<User>> ListManagersAsync();

        Task UpdateUserAsync(UserModel oUserModel);

        Task<IEnumerable<ProjectUser>> ListByProjectAsync(int projectID);

        Task<User> LoadByLoginAsync(string sLogin);

        Task<User> LoadProjectManagerAsync(int projectID);

        bool UserExists(string sLogin);

        Task<IEnumerable<HourAllocation>> ListAllocationsAsync(int periodID, int employeeID);
        Task<IEnumerable<ProjectUser>> ListUnallocatedProjectsAsync(int periodID, int employeeID);
    }
}
