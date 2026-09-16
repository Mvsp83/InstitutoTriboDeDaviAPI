using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPatrimonioTamanhoCorAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AlunoId",
                table: "BEM_PATRIMONIAL",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cor",
                table: "BEM_PATRIMONIAL",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tamanho",
                table: "BEM_PATRIMONIAL",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlunoId",
                table: "BEM_PATRIMONIAL");

            migrationBuilder.DropColumn(
                name: "Cor",
                table: "BEM_PATRIMONIAL");

            migrationBuilder.DropColumn(
                name: "Tamanho",
                table: "BEM_PATRIMONIAL");
        }
    }
}
