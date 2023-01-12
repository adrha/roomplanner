using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlanner.Infrastructure.Dbo
{
    public class RoomPlannerIdentityUserDbo : IdentityUser
    {
        [Column(TypeName = "nvarchar(255)")]
        public string FirstName { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string LastName { get; set; }

        public Guid InvitationId { get; set; }
    }
}
