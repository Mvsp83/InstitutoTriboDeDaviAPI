using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerfilProfessorSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "USUARIOS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Faixa",
                table: "USUARIOS",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoSite",
                table: "USUARIOS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MostrarNoSite",
                table: "USUARIOS",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Nome", table: "USUARIOS");
            migrationBuilder.DropColumn(name: "Faixa", table: "USUARIOS");
            migrationBuilder.DropColumn(name: "FotoSite", table: "USUARIOS");
            migrationBuilder.DropColumn(name: "MostrarNoSite", table: "USUARIOS");
        }
    }
}
