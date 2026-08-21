using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGraduacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GRADUACAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    FaixaAnterior = table.Column<int>(type: "int", nullable: false),
                    FaixaNova = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegistradoPor = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRADUACAO", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GRADUACAO_AlunoId",
                table: "GRADUACAO",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_GRADUACAO_Data_PoloId",
                table: "GRADUACAO",
                columns: new[] { "Data", "PoloId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GRADUACAO");
        }
    }
}
