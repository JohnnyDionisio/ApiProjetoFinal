using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APItoPFinal.Migrations
{
    /// <inheritdoc />
    public partial class FkRestrictSemCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompraInstrumento_Instrumentos_InstrumentosId",
                table: "CompraInstrumento");

            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Usuarios_UsuarioId",
                table: "Compras");

            migrationBuilder.DropForeignKey(
                name: "FK_Instrumentos_Categorias_IdCategoria",
                table: "Instrumentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Instrumentos_Marcas_IdMarca",
                table: "Instrumentos");

            migrationBuilder.AddForeignKey(
                name: "FK_CompraInstrumento_Instrumentos_InstrumentosId",
                table: "CompraInstrumento",
                column: "InstrumentosId",
                principalTable: "Instrumentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Usuarios_UsuarioId",
                table: "Compras",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Instrumentos_Categorias_IdCategoria",
                table: "Instrumentos",
                column: "IdCategoria",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Instrumentos_Marcas_IdMarca",
                table: "Instrumentos",
                column: "IdMarca",
                principalTable: "Marcas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompraInstrumento_Instrumentos_InstrumentosId",
                table: "CompraInstrumento");

            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Usuarios_UsuarioId",
                table: "Compras");

            migrationBuilder.DropForeignKey(
                name: "FK_Instrumentos_Categorias_IdCategoria",
                table: "Instrumentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Instrumentos_Marcas_IdMarca",
                table: "Instrumentos");

            migrationBuilder.AddForeignKey(
                name: "FK_CompraInstrumento_Instrumentos_InstrumentosId",
                table: "CompraInstrumento",
                column: "InstrumentosId",
                principalTable: "Instrumentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Usuarios_UsuarioId",
                table: "Compras",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
