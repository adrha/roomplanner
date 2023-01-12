using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlanner.Core.Entity
{
    public class RoomReservation
    {
        public Guid Id { get; set; }

        public string Subject { get; set; }

        public User User { get; set; }

        public Guid RoomId { get; set; }

        public Room Room { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }
    }
}
