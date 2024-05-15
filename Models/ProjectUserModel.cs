using System;
using System.ComponentModel.DataAnnotations;

namespace ResourceAllocationTool.Models
{
    public class ProjectUserModel
    {

        public ProjectUserModel()
        {
        }

        [Key]
        public int ID { get; set; }
        public int ProjectID { get; set; }

        [Required(ErrorMessage = "Employee Name is Required")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Project Role is Required")]
        public int RoleID { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }




    }
}
