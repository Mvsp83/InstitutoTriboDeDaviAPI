using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFotoAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoArquivoId",
                table: "INSCRICAO",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoArquivoId",
                table: "ALUNOS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CONFIG_FOTO_ALUNO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MostrarNoCadastro = table.Column<bool>(type: "bit", nullable: false),
                    MostrarNaChamada = table.Column<bool>(type: "bit", nullable: false),
                    MostrarNoResponsavel = table.Column<bool>(type: "bit", nullable: false),
                    MostrarNaCarteirinha = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONFIG_FOTO_ALUNO", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CONFIG_FOTO_ALUNO");

            migrationBuilder.DropColumn(
                name: "FotoArquivoId",
                table: "INSCRICAO");

            migrationBuilder.DropColumn(
                name: "FotoArquivoId",
                table: "ALUNOS");
        }
    }
}
