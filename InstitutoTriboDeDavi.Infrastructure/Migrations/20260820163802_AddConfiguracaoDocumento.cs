using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracaoDocumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CONFIGURACAO_DOCUMENTO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TituloCabecalho = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    LinhaExtra = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    TextoRodape = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: true),
                    MostrarLogo = table.Column<bool>(type: "bit", nullable: false),
                    MostrarDataGeracao = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONFIGURACAO_DOCUMENTO", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CONFIGURACAO_DOCUMENTO");
        }
    }
}
