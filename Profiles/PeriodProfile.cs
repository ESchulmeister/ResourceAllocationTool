using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;

using AutoMapper;

namespace ResourceAllocationTool.Profiles
{
    public class PeriodProfile : Profile
    {
        public PeriodProfile()
        {


            this.CreateMap<Period, PeriodModel>()
                .ForMember(m => m.ID, o => o.MapFrom(d => d.PerId))
                .ForMember(m => m.Name, o => o.MapFrom(d => d.PerName))
                .ForMember(m => m.WorkDays, o => o.MapFrom(d => d.PerWorkDays))
                .ForMember(m => m.WorkHours, o => o.MapFrom(d => d.PerWorkHours))
                .ForMember(m => m.IsActive, o => o.MapFrom(d => d.PerActive))

                .ReverseMap();

        }

    }
}
