using System.ComponentModel.DataAnnotations;

namespace ResourceAllocationTool.Models
{
    public class HourAllocationModel
    {
        #region Constants
        public const string KeyDelimiter = "|";
        #endregion

        public HourAllocationModel()
        {
        }

        [Key]
        public string Key
        {
            get
            {
                return $"{this.ProjectUserID}{KeyDelimiter}{this.PeriodID}";
            }
        }

        public string ID { get; set; }
        public int ProjectUserID { get; set; }

        public int PeriodID { get; set; }
        public double? EstimatedHours { get; set; } = 0;
        public double? ActualHours { get; set; } = 0;

        public double UsedHours { get; set; }

        public PeriodModel Period { get; set; }

        public ProjectUserModel ProjectUser { get; set; }
        public int UserID { get; set; }

        public string UserFullNameReversed { get; set; }

        public double? FTE { get; set; }

        public string ProjectRole { get; set; }
        public string ProjectName { get; set; }
    }
}
