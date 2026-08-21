using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAviso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AVISO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Mensagem = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PublicoAlvo = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CriadoPor = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AVISO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AVISO_CIENTE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvisoId = table.Column<long>(type: "bigint", nullable: false),
                    UsuarioLogin = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DataCiente = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AVISO_CIENTE", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AVISO_CIENTE_AvisoId_UsuarioLogin",
                table: "AVISO_CIENTE",
                columns: new[] { "AvisoId", "UsuarioLogin" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AVISO");

            migrationBuilder.DropTable(
                name: "AVISO_CIENTE");
        }
    }
}
