using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracaoDashboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CONFIGURACAO_DASHBOARD",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioLogin = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Layout = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONFIGURACAO_DASHBOARD", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CONFIGURACAO_DASHBOARD_UsuarioLogin",
                table: "CONFIGURACAO_DASHBOARD",
                column: "UsuarioLogin",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CONFIGURACAO_DASHBOARD");
        }
    }
}
