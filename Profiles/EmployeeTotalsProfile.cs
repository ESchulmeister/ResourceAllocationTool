using ResourceAllocationTool.Models;
using ResourceAllocationTool.Services;

using AutoMapper;


namespace ResourceAllocationTool
{
    public class EmployeeTotalsProfile : Profile
    {
        public EmployeeTotalsProfile()
        {
            this.CreateMap<EmployeeTotals, EmployeeTotalsModel>()
                .ForMember(m => m.FTE, o => o.MapFrom(d => d.FTE))

                .ForMember(m => m.TotalHours, o => o.MapFrom(d => d.TotalHours))

                .ForMember(m => m.UsedHours, o => o.MapFrom(d => d.UsedHours))

                .ForMember(m => m.AllocatedHours, o => o.MapFrom(d => d.AllocatedHours))

                .ForMember(m => m.RemainingHours, o => o.MapFrom(d => d.RemainingHours))

                .ReverseMap();

        }

    }
}
