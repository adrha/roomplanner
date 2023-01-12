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
    public class RoomReservationProfile : Profile
    {
        public RoomReservationProfile()
        {
            CreateMap<RoomReservationDbo, RoomReservation>();

            CreateMap<RoomReservation, RoomReservationDbo>()
                .ForMember(dst => dst.UserId, src => src.MapFrom(opt => opt.User.Id))
                .ForMember(dst => dst.User, src => src.Ignore())
                .ForMember(dst => dst.RoomId, src => src.MapFrom(opt => opt.RoomId))
                .ForMember(dst => dst.Room, src => src.Ignore())
                ;
        }
    }
}
