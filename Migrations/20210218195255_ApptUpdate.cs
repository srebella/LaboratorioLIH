using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace laberegisterLIH.Migrations
{
    public partial class ApptUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Examenes_AspNetUsers_ClienteId",
                table: "Examenes");

            migrationBuilder.DropForeignKey(
                name: "FK_Sucursales_AspNetUsers_ClienteId",
                table: "Sucursales");

            migrationBuilder.DropIndex(
                name: "IX_Sucursales_ClienteId",
                table: "Sucursales");

            migrationBuilder.DropIndex(
                name: "IX_Examenes_ClienteId",
                table: "Examenes");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Sucursales");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Examenes");

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SucursalId = table.Column<int>(type: "int", nullable: true),
                    ExamenId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClienteId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_AspNetUsers_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Examenes_ExamenId",
                        column: x => x.ExamenId,
                        principalTable: "Examenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClienteId",
                table: "Appointments",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ExamenId",
                table: "Appointments",
                column: "ExamenId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SucursalId",
                table: "Appointments",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UserId",
                table: "Appointments",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "ClienteId",
                table: "Sucursales",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClienteId",
                table: "Examenes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_ClienteId",
                table: "Sucursales",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Examenes_ClienteId",
                table: "Examenes",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Examenes_AspNetUsers_ClienteId",
                table: "Examenes",
                column: "ClienteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sucursales_AspNetUsers_ClienteId",
                table: "Sucursales",
                column: "ClienteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
