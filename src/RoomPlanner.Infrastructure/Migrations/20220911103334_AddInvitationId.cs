using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomPlanner.Infrastructure.Migrations
{
    public partial class AddInvitationId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvitationId",
                table: "AspNetUsers",
                type: "nvarchar(255)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvitationId",
                table: "AspNetUsers");
        }
    }
}
