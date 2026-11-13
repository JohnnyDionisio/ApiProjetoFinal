using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APItoPFinal.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoMovimentosEstoque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovimentosEstoque",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstrumentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Variacao = table.Column<int>(type: "int", nullable: false),
                    EstoqueApos = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CompraId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentosEstoque", x => x.Id);
                    table.CheckConstraint("CK_MovimentosEstoque_EstoqueApos", "[EstoqueApos] >= 0");
                    table.CheckConstraint("CK_MovimentosEstoque_Variacao", "[Variacao] <> 0");
                    table.ForeignKey(
                        name: "FK_MovimentosEstoque_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MovimentosEstoque_Instrumentos_InstrumentoId",
                        column: x => x.InstrumentoId,
                        principalTable: "Instrumentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovimentosEstoque_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimentosEstoque_CompraId",
                table: "MovimentosEstoque",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentosEstoque_InstrumentoId_DataHora",
                table: "MovimentosEstoque",
                columns: new[] { "InstrumentoId", "DataHora" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimentosEstoque_UsuarioId",
                table: "MovimentosEstoque",
                column: "UsuarioId");

            // saldo inicial: o estoque atual de cada instrumento vira o ponto de partida do histórico
            migrationBuilder.Sql(@"
                INSERT INTO MovimentosEstoque (Id, InstrumentoId, Tipo, Variacao, EstoqueApos, Motivo, CompraId, UsuarioId, DataHora)
                SELECT NEWID(), Id, 0, Estoque, Estoque, 'Saldo inicial (migração)', NULL, NULL, SYSUTCDATETIME()
                FROM Instrumentos
                WHERE Estoque > 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimentosEstoque");
        }
    }
}
