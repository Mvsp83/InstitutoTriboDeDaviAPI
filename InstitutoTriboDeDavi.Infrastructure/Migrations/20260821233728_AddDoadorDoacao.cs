using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDoadorDoacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DOACAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoadorId = table.Column<long>(type: "bigint", nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Forma = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Finalidade = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReciboDocumentoId = table.Column<long>(type: "bigint", nullable: true),
                    ReciboNumero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RegistradoPor = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOACAO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DOADOR",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoPessoa = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Documento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Endereco = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOADOR", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DOACAO_Data",
                table: "DOACAO",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_DOACAO_DoadorId",
                table: "DOACAO",
                column: "DoadorId");

            migrationBuilder.CreateIndex(
                name: "IX_DOADOR_Nome",
                table: "DOADOR",
                column: "Nome");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOACAO");

            migrationBuilder.DropTable(
                name: "DOADOR");
        }
    }
}
