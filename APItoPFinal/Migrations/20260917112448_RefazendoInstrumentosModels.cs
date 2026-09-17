using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APItoPFinal.Migrations
{
    /// <inheritdoc />
    public partial class RefazendoInstrumentosModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Marca",
                table: "Instrumentos");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Instrumentos");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoriaId",
                table: "Instrumentos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdCategoria",
                table: "Instrumentos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IdMarca",
                table: "Instrumentos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "MarcaId",
                table: "Instrumentos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Marca",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marca", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Instrumentos_CategoriaId",
                table: "Instrumentos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Instrumentos_MarcaId",
                table: "Instrumentos",
                column: "MarcaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instrumentos_Categoria_CategoriaId",
                table: "Instrumentos",
                column: "CategoriaId",
                principalTable: "Categoria",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Instrumentos_Marca_MarcaId",
                table: "Instrumentos",
                column: "MarcaId",
                principalTable: "Marca",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instrumentos_Categoria_CategoriaId",
                table: "Instrumentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Instrumentos_Marca_MarcaId",
                table: "Instrumentos");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Marca");

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
                name: "IdCategoria",
                table: "Instrumentos");

            migrationBuilder.DropColumn(
                name: "IdMarca",
                table: "Instrumentos");

            migrationBuilder.DropColumn(
                name: "MarcaId",
                table: "Instrumentos");

            migrationBuilder.AddColumn<string>(
                name: "Marca",
                table: "Instrumentos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Instrumentos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
