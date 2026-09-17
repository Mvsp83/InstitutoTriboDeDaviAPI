using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecadoAprovado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // defaultValue true: recados que já existiam (criados pela equipe)
            // continuam aprovados/visíveis; novos do portal são gravados como
            // false explicitamente pelo serviço.
            migrationBuilder.AddColumn<bool>(
                name: "Aprovado",
                table: "RECADO",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aprovado",
                table: "RECADO");
        }
    }
}
