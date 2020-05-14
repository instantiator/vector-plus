using Microsoft.EntityFrameworkCore.Migrations;

namespace VectorPlus.Web.Migrations
{
    public partial class AddReferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniqueReference",
                table: "Modules",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueReference",
                table: "Modules");
        }
    }
}
