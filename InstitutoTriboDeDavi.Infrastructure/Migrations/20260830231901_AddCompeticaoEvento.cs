using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompeticaoEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "COMPETICAO_EVENTO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Local = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Organizador = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PrazoInscricao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMPETICAO_EVENTO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "COMPETICAO_PARTICIPACAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompeticaoEventoId = table.Column<long>(type: "BIGINT", nullable: false),
                    AtletaId = table.Column<long>(type: "bigint", nullable: false),
                    CategoriaPeso = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Colocacao = table.Column<int>(type: "int", nullable: false),
                    Lutas = table.Column<int>(type: "int", nullable: false),
                    Vitorias = table.Column<int>(type: "int", nullable: false),
                    Finalizacoes = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMPETICAO_PARTICIPACAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_COMPETICAO_PARTICIPACAO_COMPETICAO_EVENTO_CompeticaoEventoId",
                        column: x => x.CompeticaoEventoId,
                        principalTable: "COMPETICAO_EVENTO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_COMPETICAO_PARTICIPACAO_AtletaId",
                table: "COMPETICAO_PARTICIPACAO",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_COMPETICAO_PARTICIPACAO_CompeticaoEventoId",
                table: "COMPETICAO_PARTICIPACAO",
                column: "CompeticaoEventoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "COMPETICAO_PARTICIPACAO");

            migrationBuilder.DropTable(
                name: "COMPETICAO_EVENTO");
        }
    }
}
