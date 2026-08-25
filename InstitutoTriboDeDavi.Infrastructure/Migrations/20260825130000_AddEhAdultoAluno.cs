using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using InstitutoTriboDeDavi.Infrastructure.Context;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    // Marca "é adulto" no aluno: definida na aprovação da inscrição a partir do
    // público. O [Migration] fica aqui (sem Designer) para ser aplicável por
    // `dotnet ef database update` / Migrate().
    [DbContext(typeof(TriboDeDaviContext))]
    [Migration("20260825130000_AddEhAdultoAluno")]
    public partial class AddEhAdultoAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EhAdulto",
                table: "ALUNOS",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EhAdulto",
                table: "ALUNOS");
        }
    }
}
