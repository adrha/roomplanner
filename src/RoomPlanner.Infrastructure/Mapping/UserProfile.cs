using AutoMapper;
using RoomPlanner.Core.Entity;
using RoomPlanner.Infrastructure.Dbo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlanner.Infrastructure.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RoomPlannerIdentityUserDbo, User>()
                .ForMember(dst => dst.Id, src => src.MapFrom(opt => Guid.Parse(opt.Id)))
                ;

            CreateMap<User, RoomPlannerIdentityUserDbo>()
                .ForMember(dst => dst.Id, src => src.MapFrom(opt => opt.Id.ToString()))
                ;
        }
    }
}
