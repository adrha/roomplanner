using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomPlanner.Core.Entity;
using RoomPlanner.Core.Exceptions;
using RoomPlanner.Infrastructure.Context;
using RoomPlanner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomPlanner.Infrastructure.Dbo;

namespace RoomPlanner.Infrastructure.Repositories
{
    public class RoomReservationRepository : IRoomReservationRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<RoomReservationRepository> logger;

        public RoomReservationRepository(ApplicationDbContext context, IMapper mapper, ILogger<RoomReservationRepository> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<RoomReservation> GetRoomReservationByIdAsync(Guid id)
        {
            var dbo = await context.RoomReservations.Where(r => r.Id == id).Include(u => u.User).SingleOrDefaultAsync();

            if (dbo == null)
            {
                throw new NotFoundException($"Roomreservation with ID {id} could not be found");
            }

            return mapper.Map<RoomReservation>(dbo);
        }

        public async Task CreateRoomReservationAsync(RoomReservation roomReservation)
        {
            var mapped = mapper.Map<RoomReservationDbo>(roomReservation);
            mapped.Id = Guid.NewGuid();
            await context.RoomReservations.AddAsync(mapped);
            await context.SaveChangesAsync();
        }

        public async Task UpdateReservationAsync(RoomReservation roomReservation)
        {
            var mapped = mapper.Map<RoomReservationDbo>(roomReservation);

            context.RoomReservations.Update(mapped);
            await context.SaveChangesAsync();
        }

        public async Task DeleteRoomReservationByIdAsync(Guid id)
        {
            var dbo = await context.RoomReservations.SingleOrDefaultAsync(r => r.Id == id);

            if (dbo == null)
            {
                throw new NotFoundException($"Roomreservation with ID {id} could not be found");
            }

            context.RoomReservations.Remove(dbo);
            await context.SaveChangesAsync();
        }

        public async Task<IList<RoomReservation>> GetAllRoomReservationsAsync(IList<Guid> roomIds, DateTime from, DateTime to)
        { 
            var dbos = await context.RoomReservations.Where(r => roomIds.Contains(r.RoomId)
                && ((r.From > from && r.To < to) || (r.From < from && r.To > from) || (r.From < to && r.To > to)))
                .Include(r => r.User)
                .Include(r => r.Room)
                .ToListAsync();

            return mapper.Map<IList<RoomReservation>>(dbos);
        }        
    }
}
