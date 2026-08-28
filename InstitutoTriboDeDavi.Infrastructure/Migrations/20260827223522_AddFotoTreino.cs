using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFotoTreino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FOTO_ARQUIVO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Conteudo = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FOTO_ARQUIVO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FOTO_TREINO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Turma = table.Column<int>(type: "int", nullable: false),
                    SemanaChave = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    DataReferencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Legenda = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ArquivoId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProfessorId = table.Column<long>(type: "bigint", nullable: false),
                    Publicada = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FOTO_TREINO", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FOTO_TREINO_PoloId_Turma_SemanaChave",
                table: "FOTO_TREINO",
                columns: new[] { "PoloId", "Turma", "SemanaChave" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FOTO_TREINO_Publicada",
                table: "FOTO_TREINO",
                column: "Publicada");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FOTO_ARQUIVO");

            migrationBuilder.DropTable(
                name: "FOTO_TREINO");
        }
    }
}
