using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposFichaAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Altura",
                table: "ALUNOS",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Complemento",
                table: "ALUNOS",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "ALUNOS",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Serie",
                table: "ALUNOS",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefone2",
                table: "ALUNOS",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Altura",
                table: "ALUNOS");

            migrationBuilder.DropColumn(
                name: "Complemento",
                table: "ALUNOS");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "ALUNOS");

            migrationBuilder.DropColumn(
                name: "Serie",
                table: "ALUNOS");

            migrationBuilder.DropColumn(
                name: "Telefone2",
                table: "ALUNOS");
        }
    }
}
