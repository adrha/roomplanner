using AutoMapper;
using Microsoft.Extensions.Logging;
using RoomPlanner.Core;
using RoomPlanner.Core.Entity;
using RoomPlanner.Core.Exceptions;
using RoomPlanner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlanner.Business.Services
{
    public class RoomReservationService
    {       
        private readonly IRoomRepository roomRepository;
        private readonly IRoomReservationRepository roomReservationRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly ILogger logger;

        public RoomReservationService(IRoomRepository roomRepository, IRoomReservationRepository roomReservationRepository, IUserRepository userRepository, IMapper mapper, ILogger<RoomReservationService> logger)
        {
            this.roomRepository = roomRepository;
            this.roomReservationRepository = roomReservationRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task CreateRoomReservationAsync(RoomReservation reservation)
        {
            if(reservation == null) throw new ArgumentNullException(nameof(reservation));
            if (reservation.User == null) throw new ArgumentNullException("User cannot be null");

            if (!await IsUserAdminAsync(reservation.User))
            {
                await ValidateBookingUserRoleAsync(reservation.User);
            }
            

            var room = await roomRepository.GetRoomByIdAsync(reservation.RoomId);
            reservation.Room = room;

            await ValidateRoomAvailabilityAsync(reservation);

            await roomReservationRepository.CreateRoomReservationAsync(reservation);
        }

        public async Task UpdateRoomReservationAsync(RoomReservation reservation)
        {
            if (reservation == null) throw new ArgumentNullException(nameof(reservation));
            if (reservation.User == null) throw new ArgumentNullException("User cannot be null");

            bool isAdmin = await IsUserAdminAsync(reservation.User);

            if (!isAdmin)
            {
                await ValidateBookingUserRoleAsync(reservation.User);
            }

            RoomReservation existingReservation = await roomReservationRepository.GetRoomReservationByIdAsync(reservation.Id);

            if (!isAdmin)
            {
                ValidateExistingReservationOwner(reservation.User, existingReservation);
            }            

            var room = await roomRepository.GetRoomByIdAsync(reservation.RoomId);
            reservation.Room = room;

            await ValidateRoomAvailabilityAsync(reservation);

            await roomReservationRepository.UpdateReservationAsync(reservation);
        }

        public async Task DeleteRoomReservationByIdAsync(Guid id, User callingUser)
        {
            if (callingUser == null) throw new ArgumentNullException(nameof(callingUser));

            bool isAdmin = await IsUserAdminAsync(callingUser);

            if (!isAdmin)
            {
                await ValidateBookingUserRoleAsync(callingUser);
            }

            RoomReservation existingReservation = await roomReservationRepository.GetRoomReservationByIdAsync(id);

            if (!isAdmin)
            {
                ValidateExistingReservationOwner(callingUser, existingReservation);
            }

            await roomReservationRepository.DeleteRoomReservationByIdAsync(id);
        }

        private void ValidateExistingReservationOwner(User editingUser, RoomReservation existingReservation)
        {
            if (existingReservation.User.Id != editingUser.Id)
            {
                throw new ForbiddenOperationException("The reservation to change does not belong to the calling user");
            }
        }

        private async Task<bool> IsUserAdminAsync(User editingUser)
        {
            return await userRepository.HasUserRoleAsync(editingUser.Id, UserRoles.AdministrativeRoleName);
        }

        private async Task ValidateBookingUserRoleAsync(User editingUser)
        {
            var hasRole = await userRepository.HasUserRoleAsync(editingUser.Id, UserRoles.RoomBookingRoleName);
            if (!hasRole)
            {
                throw new ForbiddenOperationException("Current user is not allowed to create or modify reservations");
            }
        }

        private async Task ValidateRoomAvailabilityAsync(RoomReservation reservation)
        {
            var existingReservationsOfRoom = await roomReservationRepository.GetAllRoomReservationsAsync(new List<Guid> { reservation.RoomId }, reservation.From, reservation.To);
            if (existingReservationsOfRoom.Where(r => r.Id != reservation.Id).Any())
            {
                throw new UnavailableException("Room is not available in this period");
            }
        }
    }
}
