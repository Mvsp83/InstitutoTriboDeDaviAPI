using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCodigoResponsavel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoResponsavel",
                table: "ALUNOS",
                type: "VARCHAR(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ALUNOS_CodigoResponsavel",
                table: "ALUNOS",
                column: "CodigoResponsavel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ALUNOS_CodigoResponsavel",
                table: "ALUNOS");

            migrationBuilder.DropColumn(
                name: "CodigoResponsavel",
                table: "ALUNOS");
        }
    }
}
