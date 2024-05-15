using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using ResourceAllocationTool.Services;
using System.Linq;


using AutoMapper;


namespace ResourceAllocationTool
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {

            this.CreateMap<Project, ProjectModel>()
                .ForMember(c => c.ID, o => o.MapFrom(c => c.PrjId))
                .ForMember(c => c.Name, o => o.MapFrom(c => c.PrjName))
                .ForMember(c => c.Description, o => o.MapFrom(c => c.PrjDesc))
                .ForMember(c => c.IsActive, o => o.MapFrom(c => c.PrjActive.Value))
                .ForMember(c => c.ManagerID, o => o.MapFrom(c => c.ProjectUsers.FirstOrDefault(pu => pu.PuRoleId == Constants.Roles.Supervisor).PuUserId))
                .ForMember(m => m.WillTrackHours, o => o.MapFrom(d => d.PrjApps.HasValue && this.WillTrack(d)))
                .ReverseMap();
        }

        private bool WillTrack(Project oProject)
        {
            int iValue = (oProject.PrjApps.HasValue) ? oProject.PrjApps.Value : 0;
            return (iValue == 0) ? false : ((iValue & Constants.ApplicationBit) == Constants.ApplicationBit);
        }

    }
}
