using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResourceAllocationTool.Services
{


    /// <summary>
    /// apiPeriods endpoints
    /// </summary>
    /// 
    public interface IPeriodRepository
    {

        Task<IEnumerable<Period>> ListAsync();

        Task<Period> GetByIDAsync(int id);

        Task SavePeriodAsync(PeriodModel oPeriodModel);

        Task DeletePeriodAsync(int periodID);


    }
}
