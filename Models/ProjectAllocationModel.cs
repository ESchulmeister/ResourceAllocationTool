using System.ComponentModel.DataAnnotations;

namespace ResourceAllocationTool.Models
{
    public class ProjectAllocationModel
    {
        #region Constants
        public const string KeyDelimiter = "|";
        #endregion

        public ProjectAllocationModel()
        {
        }

        [Key]
        public string Key
        {
            get
            {
                return $"{this.ProjectUserID.ToString()}{KeyDelimiter}{this.PeriodID.ToString()}";
            }
        }

        public string ID { get; set; }
        public int ProjectUserID { get; set; }

        public int PeriodID { get; set; }
        public double? EstimatedHours { get; set; } = 0;
        public double? ActualHours { get; set; } = 0;

        public float TotalHours { get; set; }
        public double UsedHours { get; set; }
        public double RemainingHours
        {
            get
            {
                return (double)this.TotalHours - this.UsedHours;
            }
        }

        public PeriodModel Period { get; set; }

        public ProjectUserModel ProjectUser { get; set; }
        public int UserID { get; set; }

        public string UserFullNameReversed { get; set; }

        public double? FTE { get; set; }

        public string ProjectRole { get; set; }

    }
}
