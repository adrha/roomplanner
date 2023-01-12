using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoomPlanner.Infrastructure.Dbo;

namespace RoomPlanner.Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {            
        }

        public DbSet<RoomDbo> Rooms { get; set; }

        public DbSet<RoomReservationDbo> RoomReservations { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}