using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomPlanner.App.Models.InputModels;
using RoomPlanner.App.Models.ViewModels;
using RoomPlanner.Core;
using RoomPlanner.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;

namespace RoomPlanner.App.Pages.Reservations
{
    public class IndexModel : PageModel
    {
        private readonly IRoomRepository roomRepository;
        private readonly IRoomReservationRepository roomReservationRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime Date { get; set; }
        public bool IsAllowedToBook { get; private set; } = false;
        public string TimeTableView { get; set; } = "office-hrs";
        public RoomReservationFilterInputModel Input { get; set; }
        public IList<RoomViewModel> Rooms { get; private set; }
        public IList<RoomReservationViewModel> RoomReservations { get; private set; }

        public IndexModel(IRoomRepository roomRepository, IRoomReservationRepository roomReservationRepository, IUserRepository userRepository, IMapper mapper)
        {
            this.roomRepository = roomRepository;
            this.roomReservationRepository = roomReservationRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task OnGetAsync([FromQuery]string? date, [FromQuery] string? timeTableView)
        {
            DateTime dt = DateTime.Now;
            if (date != null)
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                dt = DateTime.ParseExact(date, "dd.MM.yyyy", provider);
            }           

            DateTime from = dt.Date;
            DateTime to = dt.Date.AddHours(24);
            Date = from;

            if(timeTableView == "24hrs")
            {
                TimeTableView = "24hrs";
            }
            else
            {
                TimeTableView = "officehrs";
            }

            if(!string.IsNullOrEmpty(User?.Identity?.Name))
            {
                IsAllowedToBook = await userRepository.HasUserRoleAsync(User.Identity.Name, UserRoles.RoomBookingRoleName);
            }            

            await LoadReservationsAsync(from, to);
        }

        private async Task LoadReservationsAsync(DateTime from, DateTime to)
        {
            var rooms = await roomRepository.GetAllRoomsAsync();
            Rooms = mapper.Map<IList<RoomViewModel>>(rooms);

            

            var fakeGuids = rooms.Select(r => r.Id).ToList();
            var roomReservations = await roomReservationRepository.GetAllRoomReservationsAsync(fakeGuids, from, to);
            RoomReservations = mapper.Map<IList<RoomReservationViewModel>>(roomReservations);
        }
    }
}
