using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomPlanner.Infrastructure.Dbo
{
    public class RoomDbo
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string? Building { get; set; }

        [Column(TypeName = "int")]
        public int? Floor { get; set; }
    }
}
