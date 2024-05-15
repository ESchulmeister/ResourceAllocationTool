using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;

using AutoMapper;


namespace ResourceAllocationTool
{
    public class ProjectUserProfile : Profile
    {
        public ProjectUserProfile()
        {
            this.CreateMap<ProjectUser, ProjectUserModel>()
                .ForMember(m => m.ID, o => o.MapFrom(d => d.PuId))
                .ForMember(m => m.UserID, o => o.MapFrom(d => d.PuUserId))
                .ForMember(m => m.ProjectID, o => o.MapFrom(d => d.PuProjectId))
                .ForMember(m => m.RoleID, o => o.MapFrom(d => d.PuRoleId))
                .ForMember(m => m.CreatedBy, o => o.MapFrom(d => d.PuCreatedBy))
                .ForMember(m => m.CreatedDate, o => o.MapFrom(d => d.PuCreatedDate))
                .ForMember(m => m.ModifiedBy, o => o.MapFrom(d => d.PuModifiedBy))
                .ForMember(m => m.ModifiedDate, o => o.MapFrom(d => d.PuModifiedDate))

                .ReverseMap();

        }

    }
}
