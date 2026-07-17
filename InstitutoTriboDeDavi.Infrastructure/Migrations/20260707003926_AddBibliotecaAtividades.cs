using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBibliotecaAtividades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ATIVIDADES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Principio = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReferenciaBiblica = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATIVIDADES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ATIVIDADES_DO_BLOCO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlocoDoPlanoId = table.Column<long>(type: "BIGINT", nullable: false),
                    AtividadeId = table.Column<long>(type: "BIGINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATIVIDADES_DO_BLOCO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATIVIDADES_DO_BLOCO_ATIVIDADES_AtividadeId",
                        column: x => x.AtividadeId,
                        principalTable: "ATIVIDADES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ATIVIDADES_DO_BLOCO_BLOCOS_DO_PLANO_BlocoDoPlanoId",
                        column: x => x.BlocoDoPlanoId,
                        principalTable: "BLOCOS_DO_PLANO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ATIVIDADES_DO_BLOCO_AtividadeId",
                table: "ATIVIDADES_DO_BLOCO",
                column: "AtividadeId");

            migrationBuilder.CreateIndex(
                name: "IX_ATIVIDADES_DO_BLOCO_BlocoDoPlanoId",
                table: "ATIVIDADES_DO_BLOCO",
                column: "BlocoDoPlanoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ATIVIDADES_DO_BLOCO");

            migrationBuilder.DropTable(
                name: "ATIVIDADES");
        }
    }
}
