using AutoMapper;
using RoomPlanner.App.Models.InputModels;
using RoomPlanner.App.Models.ViewModels;
using RoomPlanner.Core.Entity;

namespace RoomPlanner.App.Mappings
{
    public class ModelsProfile : Profile
    {
        public ModelsProfile()
        {
            CreateMap<Room, RoomViewModel>();

            CreateMap<RoomReservation, RoomReservationViewModel>();

            CreateMap<RoomReservationInputModel, RoomReservation>()
                ;

            CreateMap<User, UserViewModel>();

            CreateMap<UserInputModel, User>()
                ;

            CreateMap<User, UserInputModel>()
                ;
        }
    }
}
