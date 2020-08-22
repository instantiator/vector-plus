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
                    UniqueReference = table.Column<string>(nullable: true),
                    Added = table.Column<DateTime>(nullable: false),
                    Zip = table.Column<byte[]>(nullable: true),
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

            migrationBuilder.CreateTable(
                name: "RoboConfig",
                columns: table => new
                {
                    RoboConfigId = table.Column<Guid>(nullable: false),
                    UseEnvironment = table.Column<bool>(nullable: false),
                    RobotName = table.Column<string>(nullable: true),
                    RobotSerial = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    RobotGuid = table.Column<string>(nullable: true),
                    RobotCert = table.Column<string>(nullable: true),
                    IpOverrideStr = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoboConfig", x => x.RoboConfigId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "RoboConfig");
        }
    }
}
