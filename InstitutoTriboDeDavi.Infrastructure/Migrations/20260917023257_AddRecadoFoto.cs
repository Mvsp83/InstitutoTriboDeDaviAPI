using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecadoFoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoArquivoId",
                table: "RECADO",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoArquivoId",
                table: "RECADO");
        }
    }
}
