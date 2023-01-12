using Microsoft.AspNetCore.Identity;
using RoomPlanner.Infrastructure.Context;
using RoomPlanner.Infrastructure.Dbo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalTests
{
    public class DataSeeder
    {
        public const string CleanerUserId = "730a6840-be5d-4ea2-ba52-01720888a5ec";
        public const string AdminUserId = "d8754c2f-2278-4f42-b132-bf6e9f1e3342";
        public const string BookingUserId = "1078c125-538d-46bc-ade5-94b2afdc3b0f";

        public const string Room1Id = "9bf6893b-8611-41f0-9d8f-a7090339403f";
        public const string Room2Id = "b45e7355-e698-44a9-ada7-4389d8c23a5f";

        public static async Task SeedDataAsync(ApplicationDbContext context)
        {           

            RoomPlannerIdentityUserDbo identityUser = new RoomPlannerIdentityUserDbo
            {
                Id = BookingUserId,
                UserName = "sampleuser@sampledomain.com",
                NormalizedUserName = "SAMPLEUSER@SAMPLEDOMAIN.COM",
                Email = "sampleuser@sampledomain.com",
                NormalizedEmail = "SAMPLEUSER@SAMPLEDOMAIN.COM",
                EmailConfirmed = true,
                FirstName = "Sample",
                LastName = "Man"
            };
            context.Users.Add(identityUser);

            RoomPlannerIdentityUserDbo adminUser = new RoomPlannerIdentityUserDbo
            {
                Id = AdminUserId,
                UserName = "sampleadmin@sampledomain.ch",
                NormalizedUserName = "SAMPLEADMIN@SAMPLEDOMAIN.CH",
                Email = "sampleadmin@sampledomain.ch",
                NormalizedEmail = "SAMPLEADMIN@SAMPLEDOMAIN.CH",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName= "Admin"
            };
            context.Users.Add(adminUser);

            RoomPlannerIdentityUserDbo cleanUser = new RoomPlannerIdentityUserDbo
            {
                Id = CleanerUserId,
                UserName = "samplecleaner@sampledomain.ch",
                NormalizedUserName = "SAMPLECLEANER@SAMPLEDOMAIN.CH",
                Email = "samplecleaner@sampledomain.ch",
                NormalizedEmail = "SAMPLECLEANER@SAMPLEDOMAIN.CH",
                EmailConfirmed = true,
                FirstName = "Cleaner",
                LastName = "Clean"
            };
            context.Users.Add(cleanUser);

            IdentityRole bookRole = new IdentityRole
            {
                Id = "5429774b-180d-4b5d-97e0-4ed398fa5509",
                Name = "room-booking",
                NormalizedName = "ROOM-BOOKING",
                ConcurrencyStamp = "a564f107-b846-4918-ac36-2918c715f09e"
            };
            context.Roles.Add(bookRole);

            IdentityRole adminRole= new IdentityRole
            {
                Id = "85d43080-0957-4027-9d96-b3f51f7161ed",
                Name = "admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "dcee7785-4720-4b69-86c6-3f950a6f2b38"
            };
            context.Roles.Add(adminRole);

            IdentityRole cleanRole = new IdentityRole
            {
                Id = "08b51b99-433f-4701-a264-a544a2597a19",
                Name = "cleaningplan-viewer",
                NormalizedName = "CLEANINGPLAN-VIEWER",
                ConcurrencyStamp = "51271792-9f45-4add-8782-39b99daf6f45"
            };
            context.Roles.Add(cleanRole);

            context.UserRoles.Add(new IdentityUserRole<string>
            {
                RoleId = adminRole.Id,
                UserId = adminUser.Id
            });

            context.UserRoles.Add(new IdentityUserRole<string>
            {
                RoleId = bookRole.Id,
                UserId = adminUser.Id
            });

            context.UserRoles.Add(new IdentityUserRole<string>
            {
                RoleId = bookRole.Id,
                UserId = identityUser.Id
            });

            context.UserRoles.Add(new IdentityUserRole<string>
            {
                RoleId = cleanRole.Id,
                UserId = cleanUser.Id
            });

            context.Rooms.Add(new RoomDbo
            {
                Id = Guid.Parse(Room1Id),
                Name = "Room 1",
                Building = "West",
                Floor = 3
            });

            context.Rooms.Add(new RoomDbo
            {
                Id = Guid.Parse(Room2Id),
                Name = "Room 2",
                Building = "Ost",
                Floor = 3
            });

            await context.SaveChangesAsync();
        }
    }
}
