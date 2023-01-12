using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomPlanner.App.Models.InputModels;
using RoomPlanner.Business.Services;
using RoomPlanner.Core;
using RoomPlanner.Core.Entity;
using RoomPlanner.Core.Exceptions;
using RoomPlanner.Core.Interfaces;
using RoomPlanner.Infrastructure.Dbo;

namespace RoomPlanner.App.Pages.Reservation
{
    [Authorize(Roles = UserRoles.RoomBookingRoleName)]
    public class CreateModel : PageModel
    {
        private readonly RoomReservationService roomReservationService;
        private readonly IRoomRepository roomRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly ILogger<CreateModel> logger;


        [BindProperty]
        public RoomReservationInputModel Input { get; set; }

        public Room Room { get; set; }
        public DateTime StartTimeProposal { get; set; }
        public DateTime EndTimeProposal { get; set; }

        public CreateModel(RoomReservationService roomReservationService, IRoomRepository roomRepository, IUserRepository userRepository, IMapper mapper, ILogger<CreateModel> logger)
        {
            this.roomReservationService = roomReservationService;
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IActionResult> OnGetAsync([FromQuery] Guid roomId, [FromQuery]DateTime startDate)
        {
            StartTimeProposal = startDate;
            EndTimeProposal = startDate;

            await LoadRoomNameAsync(roomId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromQuery] Guid roomId)
        {
            if (ModelState.IsValid)
            {               
                try
                {                   
                    var mappedReservation = mapper.Map<RoomReservation>(Input);
                    
                    if (!string.IsNullOrEmpty(User?.Identity?.Name))
                    {
                        mappedReservation.User = await userRepository.GetUserByUsernameAsync(User.Identity.Name);
                    }
                    else
                    {
                        throw new Exception($"Could not get username from context.");
                    }

                    mappedReservation.RoomId = roomId;
                    await roomReservationService.CreateRoomReservationAsync(mappedReservation);
                    return Redirect("/Reservations");
                }
                catch(ForbiddenOperationException ex)
                {
                    logger.LogWarning(ex, $"User with username {User?.Identity?.Name} tried to book a room but is not allowed to.");
                    return Forbid();
                }
                catch (NotFoundException ex)
                {
                    logger.LogWarning(ex, $"Ressource not found");
                    return NotFound();
                }
                catch(UnavailableException ex)
                {
                    logger.LogDebug(ex, $"Room not available in period");
                    ModelState.AddModelError("Availabilty", "Room is not available for this period");
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "Error while creating a reservation");
                }
            }

            await LoadRoomNameAsync(roomId);
            return Page();
        }

        private async Task LoadRoomNameAsync(Guid roomId)
        {
            try
            {
                Room = await roomRepository.GetRoomByIdAsync(roomId);
            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex, $"Room with ID {roomId} not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not load room");
            }
        }
    }
}
