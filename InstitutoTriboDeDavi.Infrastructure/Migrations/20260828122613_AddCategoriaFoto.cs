using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriaFoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FOTO_TREINO_PoloId_Turma_DataAula",
                table: "FOTO_TREINO");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "FOTO_TREINO",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                // "polo" para as fotos já existentes (todas eram treino de polo).
                defaultValue: "polo");

            migrationBuilder.CreateIndex(
                name: "IX_FOTO_TREINO_Categoria",
                table: "FOTO_TREINO",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_FOTO_TREINO_PoloId_Turma_DataAula",
                table: "FOTO_TREINO",
                columns: new[] { "PoloId", "Turma", "DataAula" },
                unique: true,
                filter: "[Categoria] = 'polo'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FOTO_TREINO_Categoria",
                table: "FOTO_TREINO");

            migrationBuilder.DropIndex(
                name: "IX_FOTO_TREINO_PoloId_Turma_DataAula",
                table: "FOTO_TREINO");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "FOTO_TREINO");

            migrationBuilder.CreateIndex(
                name: "IX_FOTO_TREINO_PoloId_Turma_DataAula",
                table: "FOTO_TREINO",
                columns: new[] { "PoloId", "Turma", "DataAula" },
                unique: true);
        }
    }
}
