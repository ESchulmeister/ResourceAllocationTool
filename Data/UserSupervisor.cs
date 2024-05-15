#nullable disable

namespace ResourceAllocationTool.Data
{
    public partial class UserSupervisor
    {
        public int UsId { get; set; }
        public int UsUserId { get; set; }
        public int UsUserSupervisorId { get; set; }

        public virtual User UsUser { get; set; }
        public virtual User UsUserSupervisor { get; set; }
    }
}
