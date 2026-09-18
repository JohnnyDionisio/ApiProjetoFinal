using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APItoPFinal.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeyCategoriaMarca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instrumentos_Categorias_CategoriaId",
                table: "Instrumentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Instrumentos_Marcas_MarcaId",
                table: "Instrumentos");

            migrationBuilder.DropIndex(
                name: "IX_Instrumentos_CategoriaId",
                table: "Instrumentos");

            migrationBuilder.DropIndex(
                name: "IX_Instrumentos_MarcaId",
                table: "Instrumentos");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Instrumentos");

            migrationBuilder.DropColumn(
                name: "MarcaId",
                table: "Instrumentos");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Marcas",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Compras",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CPF",
                table: "Compras",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Categorias",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Instrumentos_IdCategoria",
                table: "Instrumentos",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Instrumentos_IdMarca",
                table: "Instrumentos",
                column: "IdMarca");

            migrationBuilder.AddForeignKey(
                name: "FK_Instrumentos_Categorias_IdCategoria",
                table: "Instrumentos",
                column: "IdCategoria",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Instrumentos_Marcas_IdMarca",
                table: "Instrumentos",
                column: "IdMarca",
                principalTable: "Marcas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instrumentos_Categorias_IdCategoria",
                table: "Instrumentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Instrumentos_Marcas_IdMarca",
                table: "Instrumentos");

            migrationBuilder.DropIndex(
                name: "IX_Instrumentos_IdCategoria",
                table: "Instrumentos");

            migrationBuilder.DropIndex(
                name: "IX_Instrumentos_IdMarca",
                table: "Instrumentos");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Marcas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoriaId",
                table: "Instrumentos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MarcaId",
                table: "Instrumentos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Compras",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CPF",
                table: "Compras",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Categorias",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.CreateIndex(
                name: "IX_Instrumentos_CategoriaId",
                table: "Instrumentos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Instrumentos_MarcaId",
                table: "Instrumentos",
                column: "MarcaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instrumentos_Categorias_CategoriaId",
                table: "Instrumentos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Instrumentos_Marcas_MarcaId",
                table: "Instrumentos",
                column: "MarcaId",
                principalTable: "Marcas",
                principalColumn: "Id");
        }
    }
}
