using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;

using AutoMapper;


namespace ResourceAllocationTool
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {

            this.CreateMap<Role, RoleModel>()
                .ForMember(m => m.ID, o => o.MapFrom(d => d.RId))
                .ForMember(m => m.Name, o => o.MapFrom(d => d.RName))
                .ForMember(m => m.IsSupervisor, o => o.MapFrom(d => d.RSupervisor))
                .ForMember(m => m.IsAdministrator, o => o.MapFrom(d => d.RAdministrator))
                .ForMember(m => m.IsActive, o => o.MapFrom(d => d.RActive.HasValue))

                .ReverseMap();

        }

    }
}
