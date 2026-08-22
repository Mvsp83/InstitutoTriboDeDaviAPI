using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAutorizaImagemAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutorizaImagem",
                table: "ALUNOS",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutorizaImagem",
                table: "ALUNOS");
        }
    }
}
