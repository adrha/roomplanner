using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomPlanner.App.Models.ViewModels;
using RoomPlanner.Core.Entity;
using RoomPlanner.Core.Interfaces;

namespace RoomPlanner.App.Pages.Rooms
{
    public class IndexModel : PageModel
    {
        private readonly IRoomRepository roomRepository;
        private readonly IRoomReservationRepository roomReservationRepository;
        private readonly IMapper mapper;

        public IList<RoomViewModel> Rooms { get; private set; }

        public IndexModel(IRoomRepository roomRepository, IRoomReservationRepository roomReservationRepository, IMapper mapper)
        {
            this.roomRepository = roomRepository;
            this.roomReservationRepository = roomReservationRepository;
            this.mapper = mapper;
        }

        public async Task OnGetAsync()
        {
            var rooms = await roomRepository.GetAllRoomsAsync();
            Rooms = mapper.Map<IList<RoomViewModel>>(rooms);
        }
    }
}
