using System.ComponentModel.DataAnnotations;

namespace ResourceAllocationTool.Models
{
    public class RoleModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Role Name is Required")]
        public string Name { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsActive { get; set; } = true;
        public bool WillTrackHours { get; set; } = false;

    }
}
