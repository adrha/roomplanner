using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomPlanner.App.Models.InputModels;
using RoomPlanner.App.Models.ViewModels;
using RoomPlanner.Business.Services;
using RoomPlanner.Core;
using RoomPlanner.Core.Entity;
using RoomPlanner.Core.Exceptions;
using RoomPlanner.Core.Interfaces;

namespace RoomPlanner.App.Pages.Reservation
{
    [Authorize(Roles = $"{UserRoles.RoomBookingRoleName},{UserRoles.AdministrativeRoleName}")]
    public class EditModel : PageModel
    {
        private readonly RoomReservationService roomReservationService;
        private readonly IRoomReservationRepository roomReservationRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly ILogger<EditModel> logger;

        [BindProperty]
        public RoomReservationInputModel Input { get; set; }

        public IList<RoomViewModel> Rooms { get; set; }

        public bool IsOwner { get; private set; }

        public bool RoomUnavailableInPeriod { get; private set; }

        public UserViewModel ReservationUser { get; private set; }

        public EditModel(RoomReservationService roomReservationService, IRoomReservationRepository roomReservationRepository, IRoomRepository roomRepository,  IUserRepository userRepository, IMapper mapper, ILogger<EditModel> logger)
        {
            this.roomReservationService = roomReservationService;
            this.roomReservationRepository = roomReservationRepository;
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IActionResult> OnGetAsync([FromRoute] string reservationId)
        {
            if(string.IsNullOrEmpty(reservationId))
            {
                logger.LogWarning("No reservation-id provided");
                return BadRequest("Reservation-Id can't be null");
            }

            try
            {
                Guid id = Guid.Parse(reservationId);
                await LoadRoomsAndReservationAsync(id);
            }
            catch(NotFoundException ex)
            {
                logger.LogError(ex, $"Could not find reservation with ID {reservationId}");
                return NotFound($"Room-reservation with Id {reservationId} not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Could not load reservation with ID {reservationId}");
                return BadRequest("Could not load reservation");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostChangeAsync([FromRoute] Guid? reservationId)
        {
            if (reservationId == null)
            {
                logger.LogWarning("No reservation-id provided");
                return BadRequest("Reservation-Id can't be null");
            }

            if (ModelState.IsValid)
            {
                var mappedReservation = mapper.Map<RoomReservation>(Input);
                mappedReservation.Id = reservationId.Value;

                if (!string.IsNullOrEmpty(User?.Identity?.Name))
                {
                    mappedReservation.User = await userRepository.GetUserByUsernameAsync(User.Identity.Name);
                }
                else
                {
                    throw new Exception($"Could not get username from context.");
                }

                try
                {
                    await roomReservationService.UpdateRoomReservationAsync(mappedReservation);
                    return Redirect("/Reservations");
                }
                catch(UnavailableException)
                {
                    RoomUnavailableInPeriod = true;
                }
            }

            await LoadRoomsAndReservationAsync(reservationId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromRoute] Guid? reservationId)
        {
            if (reservationId == null)
            {
                logger.LogWarning("No reservation-id provided");
                return BadRequest("Reservation-Id can't be null");
            }

            if (!string.IsNullOrEmpty(User?.Identity?.Name))
            {
                var user = await userRepository.GetUserByUsernameAsync(User.Identity.Name);
                await roomReservationService.DeleteRoomReservationByIdAsync(reservationId.Value, user);
                return Redirect("/Reservations");
            }
            else
            {
                throw new Exception($"Could not get username from context.");
            }
        }

        private async Task LoadRoomsAndReservationAsync(Guid reservationId)
        {
            var rooms = await roomRepository.GetAllRoomsAsync();
            Rooms = mapper.Map<List<RoomViewModel>>(rooms);

            var reservation = await roomReservationRepository.GetRoomReservationByIdAsync(reservationId);
            ReservationUser = mapper.Map<UserViewModel>(reservation.User);

            Input = new RoomReservationInputModel
            {
                From = reservation.From,
                To = reservation.To,
                RoomId = reservation.RoomId,
                Subject = reservation.Subject
            };

            if (!string.IsNullOrEmpty(User?.Identity?.Name))
            {
                var user = await userRepository.GetUserByUsernameAsync(User.Identity.Name);
                var admin = await userRepository.HasUserRoleAsync(user.Id, UserRoles.AdministrativeRoleName);
                IsOwner = reservation.User.Id == user.Id || admin;
            }
            else
            {
                IsOwner = false;
            }
        }
    }
}
