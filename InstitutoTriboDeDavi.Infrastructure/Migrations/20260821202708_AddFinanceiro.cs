using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinanceiro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CONTA_FINANCEIRA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Banco = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Agencia = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Numero = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    SaldoInicial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTA_FINANCEIRA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MOVIMENTACAO_FINANCEIRA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContaId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CategoriaId = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Conciliado = table.Column<bool>(type: "bit", nullable: false),
                    Documento = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TransferenciaId = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOVIMENTACAO_FINANCEIRA", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTACAO_FINANCEIRA_ContaId",
                table: "MOVIMENTACAO_FINANCEIRA",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTACAO_FINANCEIRA_Data",
                table: "MOVIMENTACAO_FINANCEIRA",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTACAO_FINANCEIRA_TransferenciaId",
                table: "MOVIMENTACAO_FINANCEIRA",
                column: "TransferenciaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CONTA_FINANCEIRA");

            migrationBuilder.DropTable(
                name: "MOVIMENTACAO_FINANCEIRA");
        }
    }
}
