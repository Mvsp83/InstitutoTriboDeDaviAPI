using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMensalidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "COBRANCA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    PlanoId = table.Column<long>(type: "bigint", nullable: true),
                    Competencia = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Vencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PagamentoData = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PagamentoValor = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PagamentoForma = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    ContaId = table.Column<long>(type: "bigint", nullable: true),
                    MovimentacaoId = table.Column<long>(type: "bigint", nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COBRANCA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MATRICULA_FINANCEIRA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    PlanoId = table.Column<long>(type: "bigint", nullable: false),
                    DiaVencimento = table.Column<int>(type: "int", nullable: false),
                    Inicio = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DescontoTipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DescontoValor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MATRICULA_FINANCEIRA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PLANO_MENSALIDADE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OpcoesVencimento = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PLANO_MENSALIDADE", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_COBRANCA_AlunoId_Competencia",
                table: "COBRANCA",
                columns: new[] { "AlunoId", "Competencia" });

            migrationBuilder.CreateIndex(
                name: "IX_COBRANCA_Competencia",
                table: "COBRANCA",
                column: "Competencia");

            migrationBuilder.CreateIndex(
                name: "IX_MATRICULA_FINANCEIRA_AlunoId",
                table: "MATRICULA_FINANCEIRA",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_MATRICULA_FINANCEIRA_PlanoId",
                table: "MATRICULA_FINANCEIRA",
                column: "PlanoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "COBRANCA");

            migrationBuilder.DropTable(
                name: "MATRICULA_FINANCEIRA");

            migrationBuilder.DropTable(
                name: "PLANO_MENSALIDADE");
        }
    }
}
