using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceTypeAndNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "AttendanceLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "AttendanceLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "AttendanceLogs");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AttendanceLogs");
        }
    }
}
