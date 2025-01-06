using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingMicroservice.Migrations
{
    /// <inheritdoc />
    public partial class RemovedAppointmentUniqueId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentUniqueId",
                table: "Appointment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AppointmentUniqueId",
                table: "Appointment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
