using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APItoPFinal.Migrations
{
    /// <inheritdoc />
    public partial class RemoveModeloAntigoEstoque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompraInstrumento");

            migrationBuilder.DropColumn(
                name: "Disponibilidade",
                table: "Instrumentos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Disponibilidade",
                table: "Instrumentos",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompraInstrumento_InstrumentosId",
                table: "CompraInstrumento",
                column: "InstrumentosId");
        }
    }
}
