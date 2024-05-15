using System;
using System.Collections.Generic;

#nullable disable

namespace ResourceAllocationTool.Data
{
    public partial class ProjectUser
    {
        public ProjectUser()
        {
            HourAllocations = new HashSet<HourAllocation>();
        }

        public int PuId { get; set; }
        public bool? PuActive { get; set; }
        public int PuProjectId { get; set; }
        public int PuUserId { get; set; }
        public int PuRoleId { get; set; }
        public string PuCreatedBy { get; set; }
        public DateTime PuCreatedDate { get; set; }
        public string PuModifiedBy { get; set; }
        public DateTime PuModifiedDate { get; set; }

        public virtual Project PuProject { get; set; }
        public virtual User PuUser { get; set; }
        public virtual ICollection<HourAllocation> HourAllocations { get; set; }
    }
}
