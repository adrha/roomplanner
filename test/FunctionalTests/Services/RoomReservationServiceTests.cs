using Microsoft.Extensions.DependencyInjection;
using RoomPlanner.Business.Services;
using RoomPlanner.Core.Entity;
using RoomPlanner.Core.Exceptions;
using RoomPlanner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalTests.Services
{
    public class RoomReservationServiceTests : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture testServerFixture;
        private readonly RoomReservationService roomReservationService;
        private readonly IRoomReservationRepository roomReservationRepository;

        public RoomReservationServiceTests(TestServerFixture testServerFixture)
        {
            this.testServerFixture = testServerFixture;
            roomReservationService = testServerFixture.Services.GetRequiredService<RoomReservationService>();
            roomReservationRepository = testServerFixture.Services.GetRequiredService<IRoomReservationRepository>();
        }

        [Theory]
        [InlineData("Test Reservation", DataSeeder.Room1Id, DataSeeder.BookingUserId, "2099-10-10T09:00", "2099-10-10T10:00")]
        [InlineData("Test Reservation2", DataSeeder.Room2Id, DataSeeder.BookingUserId, "2099-10-10T09:00", "2099-10-10T10:00")]
        [InlineData("Test Reservation", DataSeeder.Room1Id, DataSeeder.BookingUserId, "2099-10-10T07:00", "2099-10-10T08:00")]
        public async Task CreateReservation_ReservationWithoutConflicts_ReturnCreatedReservation(string subject, string roomId, string userId, string from, string to)
        {
            // Arrange
            User user = new User
            {
                Id = Guid.Parse(userId)
            };

            RoomReservation reservation = new RoomReservation
            {
                RoomId = Guid.Parse(roomId),
                User = user,
                Subject = subject,
                From = DateTime.Parse(from),
                To = DateTime.Parse(to)
            };

            // Act
            await roomReservationService.CreateRoomReservationAsync(reservation);

            // Assert
            var reservations = await roomReservationRepository.GetAllRoomReservationsAsync(new List<Guid> { reservation.RoomId }, reservation.From.AddMinutes(-1), reservation.To.AddMinutes(1));
            Assert.Equal(1, reservations.Count);
        }

        [Theory]
        [InlineData("Test Reservation", DataSeeder.Room1Id, DataSeeder.CleanerUserId, "2099-10-10T09:00", "2099-10-10T10:00")]
        [InlineData("Test Reservation2", DataSeeder.Room2Id, DataSeeder.CleanerUserId, "2099-10-10T09:00", "2099-10-10T10:00")]
        [InlineData("Test Reservation", DataSeeder.Room1Id, DataSeeder.CleanerUserId, "2099-10-10T07:00", "2099-10-10T08:00")]
        public async Task CreateReservation_NotPermittedToBook_ThrowException(string subject, string roomId, string userId, string from, string to)
        {
            // Arrange
            User user = new User
            {
                Id = Guid.Parse(userId)
            };

            RoomReservation reservation = new RoomReservation
            {
                RoomId = Guid.Parse(roomId),
                User = user,
                Subject = subject,
                From = DateTime.Parse(from),
                To = DateTime.Parse(to)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenOperationException>(async () => await roomReservationService.CreateRoomReservationAsync(reservation));
        }

        [Fact]
        public async Task CreateReservation_UnavailableToBook_ThrowException()
        {
            // Arrange
            User user = new User
            {
                Id = Guid.Parse(DataSeeder.BookingUserId)
            };

            RoomReservation reservation1 = new RoomReservation
            {
                RoomId = Guid.Parse(DataSeeder.Room1Id),
                User = user,
                Subject = "Blocker",
                From = DateTime.Parse("2099-11-10T09:00"),
                To = DateTime.Parse("2099-11-10T14:00")
            };

            await roomReservationService.CreateRoomReservationAsync(reservation1);

            RoomReservation reservation2 = new RoomReservation
            {
                RoomId = Guid.Parse(DataSeeder.Room1Id),
                User = user,
                Subject = "Failes",
                From = DateTime.Parse("2099-11-10T11:00"),
                To = DateTime.Parse("2099-11-10T12:00")
            };

            // Act & Assert
            await Assert.ThrowsAsync<UnavailableException>(async () => await roomReservationService.CreateRoomReservationAsync(reservation2));
        }
    }
}
