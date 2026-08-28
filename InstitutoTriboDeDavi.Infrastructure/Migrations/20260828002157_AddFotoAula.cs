using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFotoAula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // As fotos de teste anteriores usavam a chave por semana, que deixa de
            // existir. Limpa antes de trocar o esquema (evita conflito no novo
            // índice único por aula e datas 0001-01-01 sem sentido). É seguro:
            // eram apenas dados de teste, reenviáveis pela tela de postar.
            migrationBuilder.Sql("DELETE FROM FOTO_TREINO;");
            migrationBuilder.Sql("DELETE FROM FOTO_ARQUIVO;");

            migrationBuilder.DropIndex(
                name: "IX_FOTO_TREINO_PoloId_Turma_SemanaChave",
                table: "FOTO_TREINO");

            migrationBuilder.DropColumn(
                name: "DataReferencia",
                table: "FOTO_TREINO");

            migrationBuilder.DropColumn(
                name: "SemanaChave",
                table: "FOTO_TREINO");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAula",
                table: "FOTO_TREINO",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "POLO_FOTO_CONFIG",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    RequerAutorizacao = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POLO_FOTO_CONFIG", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FOTO_TREINO_PoloId_Turma_DataAula",
                table: "FOTO_TREINO",
                columns: new[] { "PoloId", "Turma", "DataAula" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_POLO_FOTO_CONFIG_PoloId",
                table: "POLO_FOTO_CONFIG",
                column: "PoloId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "POLO_FOTO_CONFIG");

            migrationBuilder.DropIndex(
                name: "IX_FOTO_TREINO_PoloId_Turma_DataAula",
                table: "FOTO_TREINO");

            migrationBuilder.DropColumn(
                name: "DataAula",
                table: "FOTO_TREINO");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataReferencia",
                table: "FOTO_TREINO",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SemanaChave",
                table: "FOTO_TREINO",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FOTO_TREINO_PoloId_Turma_SemanaChave",
                table: "FOTO_TREINO",
                columns: new[] { "PoloId", "Turma", "SemanaChave" },
                unique: true);
        }
    }
}
