using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResourceAllocationTool.Services
{

    /// <summary>
    /// apiRoles endpoints
    /// </summary>

    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> ListAsync();

        Task<IEnumerable<Role>> ListNonSupervisorsAsync();

        Task SaveRoleAsync(RoleModel oRoleModel);

        Task DeleteRoleAsync(int roleID);

    }
}
