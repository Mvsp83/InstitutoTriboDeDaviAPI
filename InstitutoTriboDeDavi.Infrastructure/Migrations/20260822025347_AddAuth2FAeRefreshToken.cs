using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuth2FAeRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TotpConfirmado",
                table: "USUARIOS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TotpSecret",
                table: "USUARIOS",
                type: "VARCHAR(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "REFRESH_TOKENS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<long>(type: "BIGINT", nullable: false),
                    TokenHash = table.Column<string>(type: "VARCHAR(128)", maxLength: 128, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiraEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevogadoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REFRESH_TOKENS", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_REFRESH_TOKENS_TokenHash",
                table: "REFRESH_TOKENS",
                column: "TokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_REFRESH_TOKENS_UsuarioId",
                table: "REFRESH_TOKENS",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "REFRESH_TOKENS");

            migrationBuilder.DropColumn(
                name: "TotpConfirmado",
                table: "USUARIOS");

            migrationBuilder.DropColumn(
                name: "TotpSecret",
                table: "USUARIOS");
        }
    }
}
