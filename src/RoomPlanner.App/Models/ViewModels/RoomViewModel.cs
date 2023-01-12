namespace RoomPlanner.App.Models.ViewModels
{
    public class RoomViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Building { get; set; }

        public int? Floor { get; set; }
    }
}
