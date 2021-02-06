using Microsoft.EntityFrameworkCore.Migrations;

namespace laberegisterLIH.Migrations
{
    public partial class ClientesModelUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Sucursales_RegistrationPlaceId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RegistrationPlaceId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegistrationPlaceId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ClienteId",
                table: "Sucursales",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_ClienteId",
                table: "Sucursales",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sucursales_AspNetUsers_ClienteId",
                table: "Sucursales",
                column: "ClienteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sucursales_AspNetUsers_ClienteId",
                table: "Sucursales");

            migrationBuilder.DropIndex(
                name: "IX_Sucursales_ClienteId",
                table: "Sucursales");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Sucursales");

            migrationBuilder.AddColumn<int>(
                name: "RegistrationPlaceId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RegistrationPlaceId",
                table: "AspNetUsers",
                column: "RegistrationPlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Sucursales_RegistrationPlaceId",
                table: "AspNetUsers",
                column: "RegistrationPlaceId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
