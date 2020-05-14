using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VectorPlus.Web.Migrations
{
    public partial class AddConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "RoboConfig");
        }
    }
}
