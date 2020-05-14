using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VectorPlus.Web.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    ModuleConfigId = table.Column<Guid>(nullable: false),
                    Added = table.Column<DateTime>(nullable: false),
                    DLL = table.Column<byte[]>(nullable: true),
                    Release = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    UserEnabled = table.Column<bool>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    AuthorEmail = table.Column<string>(nullable: true),
                    ModuleWebsite = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.ModuleConfigId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modules");
        }
    }
}
