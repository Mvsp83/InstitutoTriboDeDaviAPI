using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanejamentoDeAulas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Erros",
                table: "SINCRONIZACAO_HISTORICO",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "MODELOS_DE_AULA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DuracaoTotalMinutos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MODELOS_DE_AULA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PLANOS_DE_AULA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Turma = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Objetivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataPrevista = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuracaoTotalMinutos = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AulaId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PLANOS_DE_AULA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BLOCOS_DO_MODELO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModeloDeAulaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    DuracaoMinutos = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BLOCOS_DO_MODELO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BLOCOS_DO_MODELO_MODELOS_DE_AULA_ModeloDeAulaId",
                        column: x => x.ModeloDeAulaId,
                        principalTable: "MODELOS_DE_AULA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BLOCOS_DO_PLANO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanoDeAulaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    DuracaoMinutos = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BLOCOS_DO_PLANO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BLOCOS_DO_PLANO_PLANOS_DE_AULA_PlanoDeAulaId",
                        column: x => x.PlanoDeAulaId,
                        principalTable: "PLANOS_DE_AULA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BLOCOS_DO_MODELO_ModeloDeAulaId",
                table: "BLOCOS_DO_MODELO",
                column: "ModeloDeAulaId");

            migrationBuilder.CreateIndex(
                name: "IX_BLOCOS_DO_PLANO_PlanoDeAulaId",
                table: "BLOCOS_DO_PLANO",
                column: "PlanoDeAulaId");

            migrationBuilder.CreateIndex(
                name: "IX_PLANOS_DE_AULA_PoloId_Turma_DataPrevista",
                table: "PLANOS_DE_AULA",
                columns: new[] { "PoloId", "Turma", "DataPrevista" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BLOCOS_DO_MODELO");

            migrationBuilder.DropTable(
                name: "BLOCOS_DO_PLANO");

            migrationBuilder.DropTable(
                name: "MODELOS_DE_AULA");

            migrationBuilder.DropTable(
                name: "PLANOS_DE_AULA");

            migrationBuilder.AlterColumn<string>(
                name: "Erros",
                table: "SINCRONIZACAO_HISTORICO",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
