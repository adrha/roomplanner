using RoomPlanner.Core.Entity;

namespace RoomPlanner.Core.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room> GetRoomByIdAsync(Guid id);

        Task<IList<Room>> GetAllRoomsAsync();

        Task CreateRoomAsync(Room room);

        Task DeleteRoomByIdAsync(Guid id);
    }
}
