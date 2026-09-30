using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APItoPFinal.Migrations
{
    /// <inheritdoc />
    public partial class FixCompraInstrumentoRelacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Instrumentos_InstrumentoId",
                table: "Compras");

            migrationBuilder.DropIndex(
                name: "IX_Compras_InstrumentoId",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "InstrumentoId",
                table: "Compras");

            migrationBuilder.CreateTable(
                name: "CompraInstrumento",
                columns: table => new
                {
                    ComprasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstrumentosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompraInstrumento", x => new { x.ComprasId, x.InstrumentosId });
                    table.ForeignKey(
                        name: "FK_CompraInstrumento_Compras_ComprasId",
                        column: x => x.ComprasId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompraInstrumento_Instrumentos_InstrumentosId",
                        column: x => x.InstrumentosId,
                        principalTable: "Instrumentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompraInstrumento_InstrumentosId",
                table: "CompraInstrumento",
                column: "InstrumentosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompraInstrumento");

            migrationBuilder.AddColumn<Guid>(
                name: "InstrumentoId",
                table: "Compras",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Compras_InstrumentoId",
                table: "Compras",
                column: "InstrumentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Instrumentos_InstrumentoId",
                table: "Compras",
                column: "InstrumentoId",
                principalTable: "Instrumentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
