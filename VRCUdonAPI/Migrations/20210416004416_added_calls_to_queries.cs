using Microsoft.EntityFrameworkCore.Migrations;

namespace VRCUdonAPI.Migrations
{
    public partial class added_calls_to_queries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Calls",
                table: "Queries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calls",
                table: "Queries");
        }
    }
}
