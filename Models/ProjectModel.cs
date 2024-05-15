using System.ComponentModel.DataAnnotations;

namespace ResourceAllocationTool.Models
{
    public class ProjectModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool? WillTrackHours { get; set; } = false;

        [Required(ErrorMessage = "Manager Name Required")]
        public int? ManagerID { get; set; }

    }
}
