using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using InstitutoTriboDeDavi.Infrastructure.Context;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    // Permissão por usuário: libera o professor a acessar o módulo Programa de
    // Graduação. O atributo [Migration] é declarado aqui (sem arquivo Designer)
    // para que a migração seja aplicável por `dotnet ef database update`.
    [DbContext(typeof(TriboDeDaviContext))]
    [Migration("20260824230000_AddPermiteGraduacaoUsuario")]
    public partial class AddPermiteGraduacaoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PermiteGraduacao",
                table: "USUARIOS",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PermiteGraduacao",
                table: "USUARIOS");
        }
    }
}
