using System.Data;

namespace ResourceAllocationTool.Services
{
    /// <summary>
    /// Data class - sp/code calculated values
    /// </summary>
    public class EmployeeTotals
    {

        #region Properties
        public double FTE { get; set; }
        public double TotalHours { get; set; }
        public double AllocatedHours { get; set; }
        public double RemainingHours { get; set; }
        public double UsedHours { get; set; }
        #endregion

        #region Constructors
        public EmployeeTotals(IDataReader dbDataReader)
        {
            this.FTE = (double)dbDataReader["FTE"];
            this.TotalHours = (double)dbDataReader["TotalHours"];
            this.AllocatedHours = (double)dbDataReader["HoursAllocated"];
            this.RemainingHours = (double)dbDataReader["HoursRemaining"];
            this.UsedHours = (double)dbDataReader["HoursUsed"];
        }

        public EmployeeTotals()
        {
        }
        #endregion
    }
}
