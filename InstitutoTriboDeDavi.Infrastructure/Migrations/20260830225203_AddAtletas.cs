using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAtletas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ATLETA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    CategoriaPeso = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Objetivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ATLETA_ANOTACAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AtletaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Autor = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA_ANOTACAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATLETA_ANOTACAO_ATLETA_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "ATLETA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ATLETA_AVALIACAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AtletaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA_AVALIACAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATLETA_AVALIACAO_ATLETA_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "ATLETA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ATLETA_COMPETICAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AtletaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Evento = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CategoriaPeso = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Colocacao = table.Column<int>(type: "int", nullable: false),
                    Lutas = table.Column<int>(type: "int", nullable: false),
                    Vitorias = table.Column<int>(type: "int", nullable: false),
                    Finalizacoes = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA_COMPETICAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATLETA_COMPETICAO_ATLETA_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "ATLETA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ATLETA_META",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AtletaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Prazo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA_META", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATLETA_META_ATLETA_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "ATLETA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ATLETA_AVALIACAO_INDICADOR",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvaliacaoFisicaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA_AVALIACAO_INDICADOR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATLETA_AVALIACAO_INDICADOR_ATLETA_AVALIACAO_AvaliacaoFisicaId",
                        column: x => x.AvaliacaoFisicaId,
                        principalTable: "ATLETA_AVALIACAO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_AlunoId",
                table: "ATLETA",
                column: "AlunoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_ANOTACAO_AtletaId",
                table: "ATLETA_ANOTACAO",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_AVALIACAO_AtletaId",
                table: "ATLETA_AVALIACAO",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_AVALIACAO_INDICADOR_AvaliacaoFisicaId",
                table: "ATLETA_AVALIACAO_INDICADOR",
                column: "AvaliacaoFisicaId");

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_COMPETICAO_AtletaId",
                table: "ATLETA_COMPETICAO",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_META_AtletaId",
                table: "ATLETA_META",
                column: "AtletaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ATLETA_ANOTACAO");

            migrationBuilder.DropTable(
                name: "ATLETA_AVALIACAO_INDICADOR");

            migrationBuilder.DropTable(
                name: "ATLETA_COMPETICAO");

            migrationBuilder.DropTable(
                name: "ATLETA_META");

            migrationBuilder.DropTable(
                name: "ATLETA_AVALIACAO");

            migrationBuilder.DropTable(
                name: "ATLETA");
        }
    }
}
