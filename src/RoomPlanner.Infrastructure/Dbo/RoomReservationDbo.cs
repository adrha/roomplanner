using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomPlanner.Infrastructure.Dbo
{
    public class RoomReservationDbo
    {
        [Required]
        public Guid Id { get; set; }

        public string Subject { get; set; }
        
        public virtual RoomPlannerIdentityUserDbo User { get; set; }

        [Column(TypeName = "varchar(255)")]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public virtual RoomDbo Room { get; set; }

        [ForeignKey(nameof(Room))]
        public Guid RoomId { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }
    }
}
