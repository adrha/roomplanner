using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomPlanner.Infrastructure.Migrations
{
    public partial class AppendReservationSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "RoomReservations",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "RoomReservations");
        }
    }
}
