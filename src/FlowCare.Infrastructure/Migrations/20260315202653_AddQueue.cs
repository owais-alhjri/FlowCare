using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Queue",
                table: "Appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Queue",
                table: "Appointments");
        }
    }
}
