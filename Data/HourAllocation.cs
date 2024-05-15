using System;

#nullable disable

namespace ResourceAllocationTool.Data
{
    public partial class HourAllocation
    {
        public int HaId { get; set; }
        public int HapuId { get; set; }
        public int HaPeriodId { get; set; }
        public double? HaEstimatedHours { get; set; }
        public double? HaActualHours { get; set; }
        public string HaCreatedBy { get; set; }
        public string HaModifiedBy { get; set; }
        public DateTime HaCreatedDate { get; set; }
        public DateTime HaModifiedDate { get; set; }

        public virtual Period HaPeriod { get; set; }
        public virtual ProjectUser Hapu { get; set; }
    }
}
