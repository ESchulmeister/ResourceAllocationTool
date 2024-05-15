using System;
using System.Collections.Generic;

#nullable disable

namespace ResourceAllocationTool.Data
{
    public partial class Project
    {
        public Project()
        {
            ProjectUsers = new HashSet<ProjectUser>();
        }

        public int PrjId { get; set; }
        public string PrjName { get; set; }
        public int? PrjCusId { get; set; }
        public string PrjDesc { get; set; }
        public int? PrjFlags { get; set; }
        public int? PrjTxdId { get; set; }
        public string PrjFolderName { get; set; }
        public int? PrjApps { get; set; }
        public string PrjWorkingDir { get; set; }
        public string PrjRemoteWorkingDir { get; set; }
        public string PrjEntityFile { get; set; }
        public string PrjEntityRegex { get; set; }
        public string PrjArtNumMask { get; set; }
        public string PrjArtFileMask { get; set; }
        public string PrjArtFolder { get; set; }
        public string PrjMasterArtFolder { get; set; }
        public string PrjCopytoRoot { get; set; }
        public string PrjCsdbName { get; set; }
        public int? PrjStatus { get; set; }
        public int? PrjStatusUsrId { get; set; }
        public DateTime? PrjStatusDate { get; set; }
        public double? PrjPublisherVersion { get; set; }
        public string PrjCreatedBy { get; set; }
        public DateTime? PrjCreatedDate { get; set; }
        public string PrjModifiedBy { get; set; }
        public DateTime? PrjModifiedDate { get; set; }
        public byte? PrjActive { get; set; }
        public byte[] PrjModifiedTimestamp { get; set; }

        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
    }
}
