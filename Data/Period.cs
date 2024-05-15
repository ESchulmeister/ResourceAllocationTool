using System;
using System.Collections.Generic;

#nullable disable

namespace ResourceAllocationTool.Data
{
    public partial class Period
    {
        public Period()
        {
            HourAllocations = new HashSet<HourAllocation>();
        }

        public int PerId { get; set; }
        public string PerName { get; set; }
        public bool? PerActive { get; set; }
        public double PerWorkDays { get; set; }
        public double PerWorkHours { get; set; }
        public string PerCreatedBy { get; set; }
        public DateTime PerCreatedDate { get; set; }
        public string PerModifiedBy { get; set; }
        public DateTime PerModifiedDate { get; set; }

        public virtual ICollection<HourAllocation> HourAllocations { get; set; }
    }
}
