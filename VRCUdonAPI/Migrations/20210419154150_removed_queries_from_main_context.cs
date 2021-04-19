using Microsoft.EntityFrameworkCore.Migrations;

namespace VRCUdonAPI.Migrations
{
    public partial class removed_queries_from_main_context : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Queries",
                table: "Queries");

            migrationBuilder.RenameTable(
                name: "Queries",
                newName: "Query");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Query",
                table: "Query",
                column: "Address");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Query",
                table: "Query");

            migrationBuilder.RenameTable(
                name: "Query",
                newName: "Queries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Queries",
                table: "Queries",
                column: "Address");
        }
    }
}
