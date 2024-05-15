using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;

using AutoMapper;


namespace ResourceAllocationTool
{
    public class HourAllocationProfile : Profile
    {
        public HourAllocationProfile()
        {
            this.CreateMap<HourAllocation, HourAllocationModel>()
                .ForMember(m => m.ID, o => o.MapFrom(d => d.HaId))

                .ForMember(m => m.PeriodID, o => o.MapFrom(d => d.HaPeriodId))

                .ForMember(m => m.ProjectUserID, o => o.MapFrom(d => d.HapuId))

                .ForMember(m => m.ActualHours, o => o.MapFrom(d => (d.HaActualHours.HasValue) ? d.HaActualHours.Value : 0))
                .ForMember(m => m.EstimatedHours, o => o.MapFrom(d => (d.HaEstimatedHours.HasValue) ? d.HaEstimatedHours.Value : 0))
                .ForMember(m => m.UserID, o => o.MapFrom(d => d.Hapu.PuUserId))
                .ForMember(m => m.ProjectUser, o => o.MapFrom(d => d.Hapu))

                .ReverseMap();

        }

    }
}
