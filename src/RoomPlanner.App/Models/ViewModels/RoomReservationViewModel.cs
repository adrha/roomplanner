namespace RoomPlanner.App.Models.ViewModels
{
    public class RoomReservationViewModel
    {
        public Guid Id { get; set; }

        public string Subject { get; set; }

        public UserViewModel User { get; set; }

        public RoomViewModel Room { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }
    }
}
