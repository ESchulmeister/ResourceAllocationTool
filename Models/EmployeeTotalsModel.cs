namespace ResourceAllocationTool.Models
{
    public class EmployeeTotalsModel
    {
        public EmployeeTotalsModel()
        {
        }

        public double FTE { get; set; }

        public double TotalHours { get; set; }
        public double AllocatedHours { get; set; }
        public double RemainingHours { get; set; }
        public double UsedHours { get; set; }
    }
}
