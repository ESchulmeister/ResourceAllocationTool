using System.ComponentModel.DataAnnotations;

namespace ResourceAllocationTool.Models
{
    public class PeriodModel
    {

        public int ID { get; set; }

        [Required(ErrorMessage = "Period name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Work Days # is Required")]
        public double? WorkDays { get; set; }

        [Required(ErrorMessage = "Work Hours # is Required")]
        public double? WorkHours { get; set; }

        public bool IsActive { get; set; } = true;

    }
}
