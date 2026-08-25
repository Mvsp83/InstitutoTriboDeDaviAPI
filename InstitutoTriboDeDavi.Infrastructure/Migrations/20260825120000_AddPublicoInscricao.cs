using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using InstitutoTriboDeDavi.Infrastructure.Context;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    // Público da ficha de inscrição: 0 = criança/adolescente, 1 = adulto.
    // O atributo [Migration] é declarado aqui (sem arquivo Designer) para que a
    // migração seja aplicável por `dotnet ef database update` / Migrate().
    [DbContext(typeof(TriboDeDaviContext))]
    [Migration("20260825120000_AddPublicoInscricao")]
    public partial class AddPublicoInscricao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Publico",
                table: "INSCRICAO",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Publico",
                table: "INSCRICAO");
        }
    }
}
