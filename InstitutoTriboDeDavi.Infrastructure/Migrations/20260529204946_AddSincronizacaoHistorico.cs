using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSincronizacaoHistorico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SINCRONIZACAO_HISTORICO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataExecucao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PoloNome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Inseridos = table.Column<int>(type: "int", nullable: false),
                    Atualizados = table.Column<int>(type: "int", nullable: false),
                    Ignorados = table.Column<int>(type: "int", nullable: false),
                    Sucesso = table.Column<bool>(type: "bit", nullable: false),
                    Erros = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Origem = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SINCRONIZACAO_HISTORICO", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SINCRONIZACAO_HISTORICO");
        }
    }
}
