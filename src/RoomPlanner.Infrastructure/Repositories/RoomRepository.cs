using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomPlanner.Core.Entity;
using RoomPlanner.Core.Exceptions;
using RoomPlanner.Infrastructure;
using RoomPlanner.Infrastructure.Context;
using RoomPlanner.Infrastructure.Dbo;
using RoomPlanner.Core.Interfaces;

namespace RoomPlanner.Infrastructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<RoomRepository> logger;

        public RoomRepository(ApplicationDbContext context, IMapper mapper, ILogger<RoomRepository> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Room> GetRoomByIdAsync(Guid id)
        {
            var dbo = await context.Rooms.SingleOrDefaultAsync(r => r.Id == id);

            if(dbo == null)
            {
                throw new NotFoundException($"Room with ID {id} could not be found");
            }

            return mapper.Map<Room>(dbo);
        }

        public async Task<IList<Room>> GetAllRoomsAsync()
        {
            var dbos = await context.Rooms.ToListAsync();
            return mapper.Map<IList<Room>>(dbos);
        }

        public async Task CreateRoomAsync(Room room)
        {
            var dbo = mapper.Map<RoomDbo>(room);
            await context.Rooms.AddAsync(dbo);
            await context.SaveChangesAsync();
        }

        public async Task DeleteRoomByIdAsync(Guid id)
        {
            var room = await context.Rooms.SingleAsync(r => r.Id == id);

            if (room == null)
            {
                throw new NotFoundException($"Room with ID {id} could not be found");
            }

            context.Rooms.Remove(room);
            await context.SaveChangesAsync();
        }
    }
}
