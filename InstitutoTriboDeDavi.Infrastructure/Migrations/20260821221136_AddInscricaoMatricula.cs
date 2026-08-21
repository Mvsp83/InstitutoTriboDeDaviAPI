using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInscricaoMatricula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "INSCRICAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Turma = table.Column<int>(type: "int", nullable: true),
                    JaEraAluno = table.Column<bool>(type: "bit", nullable: false),
                    TurmaAnterior = table.Column<int>(type: "int", nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rg = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Cpf = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Peso = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    Altura = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    Faixa = table.Column<int>(type: "int", nullable: false),
                    Escola = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Serie = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Periodo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Parentesco = table.Column<int>(type: "int", nullable: false),
                    ParentescoOutro = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    NomeResponsavel = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    RgResponsavel = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CpfResponsavel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Rua = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Complemento = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    WhatsApp = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Telefone2 = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    RespostasSaudeJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RespostasFamiliarJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemRestricaoMedica = table.Column<bool>(type: "bit", nullable: false),
                    Medicamentos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AceitouTermo = table.Column<bool>(type: "bit", nullable: false),
                    AceitouImagem = table.Column<bool>(type: "bit", nullable: false),
                    AceitouComodato = table.Column<bool>(type: "bit", nullable: false),
                    AceitouLgpd = table.Column<bool>(type: "bit", nullable: false),
                    NomeAssinatura = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    VersaoTermos = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DataEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AlunoId = table.Column<long>(type: "bigint", nullable: true),
                    DataRevisao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevisadoPor = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ObservacaoRevisao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INSCRICAO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MATRICULA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Turma = table.Column<int>(type: "int", nullable: false),
                    InscricaoId = table.Column<long>(type: "bigint", nullable: true),
                    DataMatricula = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativa = table.Column<bool>(type: "bit", nullable: false),
                    DataEncerramento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoEncerramento = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MATRICULA", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_INSCRICAO_PoloId",
                table: "INSCRICAO",
                column: "PoloId");

            migrationBuilder.CreateIndex(
                name: "IX_INSCRICAO_Status_Ano",
                table: "INSCRICAO",
                columns: new[] { "Status", "Ano" });

            migrationBuilder.CreateIndex(
                name: "IX_MATRICULA_AlunoId_Ano",
                table: "MATRICULA",
                columns: new[] { "AlunoId", "Ano" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MATRICULA_Ano_PoloId",
                table: "MATRICULA",
                columns: new[] { "Ano", "PoloId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "INSCRICAO");

            migrationBuilder.DropTable(
                name: "MATRICULA");
        }
    }
}
