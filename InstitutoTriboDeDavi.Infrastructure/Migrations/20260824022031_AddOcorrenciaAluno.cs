using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOcorrenciaAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OCORRENCIA_ALUNO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistradoPor = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OCORRENCIA_ALUNO", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OCORRENCIA_ALUNO_AlunoId",
                table: "OCORRENCIA_ALUNO",
                column: "AlunoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OCORRENCIA_ALUNO");
        }
    }
}
