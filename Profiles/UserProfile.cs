using ResourceAllocationTool.Data;
using ResourceAllocationTool.Models;
using ResourceAllocationTool.Services;

using AutoMapper;



namespace ResourceAllocationTool
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            this.CreateMap<User, UserModel>()
                .ForMember(m => m.ID, o => o.MapFrom(d => d.UsrId))
                .ForMember(m => m.FirstName, o => o.MapFrom(d => d.UsrFirst))
                .ForMember(m => m.LastName, o => o.MapFrom(d => d.UsrLast))
                .ForMember(m => m.Login, o => o.MapFrom(d => d.UsrLogin))
                .ForMember(m => m.IsActive, o => o.MapFrom(d => d.UsrActive.HasValue))
                .ForMember(m => m.FTE, o => o.MapFrom(d => d.UsrFte))
                .ForMember(m => m.RoleID, o => o.MapFrom(d => d.UsrDefaultRole))
                .ForMember(m => m.Clock, o => o.MapFrom(d => d.UsrClock))
                .ForMember(m => m.WillTrackHours, o => o.MapFrom(d => d.UsrApps.HasValue && this.WillTrack(d)))
                .ForMember(m => m.Role, o => o.MapFrom(d => d.UsrDefaultRoleNavigation))
                .ReverseMap();

        }

        private bool WillTrack(User oUser)
        {
            int iValue = (oUser.UsrApps.HasValue) ? oUser.UsrApps.Value : 0;

            return (iValue == 0) ? false : ((iValue & Constants.ApplicationBit) == Constants.ApplicationBit);
        }
    }
}
