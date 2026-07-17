using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRelatoriosSalvos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RELATORIOS_SALVOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioLogin = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FonteId = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Colunas = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Turma = table.Column<int>(type: "int", nullable: true),
                    PoloId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RELATORIOS_SALVOS", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RELATORIOS_SALVOS_UsuarioLogin",
                table: "RELATORIOS_SALVOS",
                column: "UsuarioLogin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RELATORIOS_SALVOS");
        }
    }
}
