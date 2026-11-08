using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APItoPFinal.Migrations
{
    /// <inheritdoc />
    public partial class AddEstoqueECompraItens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estoque",
                table: "Instrumentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CompraItens",
                columns: table => new
                {
                    CompraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstrumentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompraItens", x => new { x.CompraId, x.InstrumentoId });
                    table.CheckConstraint("CK_CompraItens_Quantidade", "[Quantidade] > 0");
                    table.ForeignKey(
                        name: "FK_CompraItens_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompraItens_Instrumentos_InstrumentoId",
                        column: x => x.InstrumentoId,
                        principalTable: "Instrumentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Instrumentos_Estoque",
                table: "Instrumentos",
                sql: "[Estoque] >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_CompraItens_InstrumentoId",
                table: "CompraItens",
                column: "InstrumentoId");
            // copia o modelo antigo para o novo
            // 1) instrumento disponível vira estoque 1; indisponível vira 0
            migrationBuilder.Sql(@"
                UPDATE Instrumentos
                SET Estoque = CASE WHEN Disponibilidade = 1 THEN 1 ELSE 0 END;");

            // 2) cada ligação antiga compra-instrumento vira um item de quantidade 1,
            //    com o preço atual do instrumento (o único disponível para compras antigas)
            migrationBuilder.Sql(@"
                INSERT INTO CompraItens (CompraId, InstrumentoId, Quantidade, PrecoUnitario)
                SELECT ci.ComprasId, ci.InstrumentosId, 1, i.Preco
                FROM CompraInstrumento ci
                JOIN Instrumentos i ON i.Id = ci.InstrumentosId;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompraItens");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Instrumentos_Estoque",
                table: "Instrumentos");

            migrationBuilder.DropColumn(
                name: "Estoque",
                table: "Instrumentos");
        }
    }
}
