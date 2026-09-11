using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APItoPFinal.Migrations
{
    /// <inheritdoc />
    public partial class CorrecaoInstrumentoModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descrição",
                table: "Instrumentos",
                newName: "Descricao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "Instrumentos",
                newName: "Descrição");
        }
    }
}
