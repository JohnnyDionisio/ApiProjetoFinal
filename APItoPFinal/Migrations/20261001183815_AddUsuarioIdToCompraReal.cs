using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APItoPFinal.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioIdToCompraReal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "Compras",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Compras_UsuarioId",
                table: "Compras",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Usuarios_UsuarioId",
                table: "Compras",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Usuarios_UsuarioId",
                table: "Compras");

            migrationBuilder.DropIndex(
                name: "IX_Compras_UsuarioId",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Compras");
        }
    }
}
