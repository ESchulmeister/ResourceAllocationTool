using System;
using System.Collections.Generic;

#nullable disable

namespace ResourceAllocationTool.Data
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public int RId { get; set; }
        public string RName { get; set; }
        public bool RSupervisor { get; set; }
        public bool RAdministrator { get; set; }
        public bool? RActive { get; set; }
        public DateTime RCreatedDate { get; set; }
        public string RCreatedBy { get; set; }
        public DateTime RModifiedDate { get; set; }
        public string RModifiedBy { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
