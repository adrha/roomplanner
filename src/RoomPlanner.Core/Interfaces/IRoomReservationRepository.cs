using RoomPlanner.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlanner.Core.Interfaces
{
    public interface IRoomReservationRepository
    {
        Task<RoomReservation> GetRoomReservationByIdAsync(Guid id);

        Task<IList<RoomReservation>> GetAllRoomReservationsAsync(IList<Guid> roomIds, DateTime from, DateTime to);

        Task CreateRoomReservationAsync(RoomReservation roomReservation);

        Task DeleteRoomReservationByIdAsync(Guid id);

        Task UpdateReservationAsync(RoomReservation roomReservation);
    }
}
