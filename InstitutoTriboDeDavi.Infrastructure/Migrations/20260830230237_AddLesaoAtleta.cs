using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLesaoAtleta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ATLETA_LESAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AtletaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Local = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Gravidade = table.Column<int>(type: "int", nullable: false),
                    DataRetorno = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Recuperado = table.Column<bool>(type: "bit", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA_LESAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATLETA_LESAO_ATLETA_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "ATLETA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_LESAO_AtletaId",
                table: "ATLETA_LESAO",
                column: "AtletaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ATLETA_LESAO");
        }
    }
}
