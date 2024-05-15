using System;
using System.Collections.Generic;

#nullable disable

namespace ResourceAllocationTool.Data
{
    public partial class User
    {
        public User()
        {
            ProjectUsers = new HashSet<ProjectUser>();
            UserSupervisorUsUserSupervisors = new HashSet<UserSupervisor>();
            UserSupervisorUsUsers = new HashSet<UserSupervisor>();
        }

        public int UsrId { get; set; }
        public string UsrLogin { get; set; }
        public string UsrFirst { get; set; }
        public string UsrLast { get; set; }
        public string UsrPassword { get; set; }
        public int UsrFlags { get; set; }
        public int? UsrClock { get; set; }
        public int? UsrApps { get; set; }
        public string UsrCsdbName { get; set; }
        public string UsrEmail { get; set; }
        public string UsrCreatedBy { get; set; }
        public DateTime? UsrCreatedDate { get; set; }
        public string UsrModifiedBy { get; set; }
        public DateTime? UsrModifiedDate { get; set; }
        public byte? UsrActive { get; set; }
        public byte[] UsrModifiedTimestamp { get; set; }
        public string UsrNetLogin { get; set; }
        public string UsrSettings { get; set; }
        public int? UsrDefaultRole { get; set; }
        public double? UsrFte { get; set; }

        public virtual Role UsrDefaultRoleNavigation { get; set; }
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
        public virtual ICollection<UserSupervisor> UserSupervisorUsUserSupervisors { get; set; }
        public virtual ICollection<UserSupervisor> UserSupervisorUsUsers { get; set; }
    }
}
