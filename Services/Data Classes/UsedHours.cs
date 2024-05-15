using System.Data;

namespace ResourceAllocationTool.Services
{
    public class UsedHours
    {
        #region Properties
        public int UserID { get; set; }
        public double Hours { get; set; }
        #endregion

        #region Constructors
        public UsedHours(IDataReader dbDataReader)
        {
            this.UserID = (int)dbDataReader["puUserID"];
            this.Hours = (double)dbDataReader["HoursUsed"];
        }
        #endregion
    }
}
