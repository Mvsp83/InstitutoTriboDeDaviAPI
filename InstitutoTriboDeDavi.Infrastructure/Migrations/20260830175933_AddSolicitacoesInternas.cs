using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSolicitacoesInternas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SOLICITACAO_INTERNA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Assunto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Categoria = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: true),
                    PoloNome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    CriadoPorLogin = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    CriadoPorRole = table.Column<int>(type: "int", nullable: false),
                    DestinatarioLogin = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOLICITACAO_INTERNA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SOLICITACAO_MENSAGEM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitacaoInternaId = table.Column<long>(type: "BIGINT", nullable: false),
                    AutorLogin = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    AutorRole = table.Column<int>(type: "int", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOLICITACAO_MENSAGEM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SOLICITACAO_MENSAGEM_SOLICITACAO_INTERNA_SolicitacaoInternaId",
                        column: x => x.SolicitacaoInternaId,
                        principalTable: "SOLICITACAO_INTERNA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SOLICITACAO_INTERNA_CriadoPorLogin",
                table: "SOLICITACAO_INTERNA",
                column: "CriadoPorLogin");

            migrationBuilder.CreateIndex(
                name: "IX_SOLICITACAO_INTERNA_DestinatarioLogin",
                table: "SOLICITACAO_INTERNA",
                column: "DestinatarioLogin");

            migrationBuilder.CreateIndex(
                name: "IX_SOLICITACAO_MENSAGEM_SolicitacaoInternaId",
                table: "SOLICITACAO_MENSAGEM",
                column: "SolicitacaoInternaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SOLICITACAO_MENSAGEM");

            migrationBuilder.DropTable(
                name: "SOLICITACAO_INTERNA");
        }
    }
}
