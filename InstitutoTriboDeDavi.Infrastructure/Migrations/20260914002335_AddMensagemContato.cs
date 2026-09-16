using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMensagemContato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MENSAGEM_CONTATO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Email = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    Telefone = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Assunto = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    Mensagem = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Lida = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MENSAGEM_CONTATO", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MENSAGEM_CONTATO_DataCriacao",
                table: "MENSAGEM_CONTATO",
                column: "DataCriacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MENSAGEM_CONTATO");
        }
    }
}
