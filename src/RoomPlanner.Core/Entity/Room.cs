using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlanner.Core.Entity
{
    public class Room
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Building { get; set; }

        public int? Floor { get; set; }

        List<RoomReservation>? RoomReservations { get; set; }
    }
}
