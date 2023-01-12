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
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<RoomDbo, Room>();

            CreateMap<Room, RoomDbo>();            
        }
    }
}
