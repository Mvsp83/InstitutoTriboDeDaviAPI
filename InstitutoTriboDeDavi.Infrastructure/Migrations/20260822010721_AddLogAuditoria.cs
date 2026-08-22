using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLogAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LOG_AUDITORIA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioLogin = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Acao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Entidade = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    EntidadeId = table.Column<long>(type: "bigint", nullable: false),
                    Resumo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Alteracoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ip = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOG_AUDITORIA", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LOG_AUDITORIA_Data",
                table: "LOG_AUDITORIA",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_LOG_AUDITORIA_Entidade_EntidadeId",
                table: "LOG_AUDITORIA",
                columns: new[] { "Entidade", "EntidadeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LOG_AUDITORIA");
        }
    }
}
