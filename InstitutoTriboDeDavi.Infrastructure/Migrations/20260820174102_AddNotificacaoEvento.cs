using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificacaoEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiasAntecedencia",
                table: "EVENTO_CALENDARIO",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmailsNotificacao",
                table: "EVENTO_CALENDARIO",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificacaoEnviada",
                table: "EVENTO_CALENDARIO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Notificar",
                table: "EVENTO_CALENDARIO",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiasAntecedencia",
                table: "EVENTO_CALENDARIO");

            migrationBuilder.DropColumn(
                name: "EmailsNotificacao",
                table: "EVENTO_CALENDARIO");

            migrationBuilder.DropColumn(
                name: "NotificacaoEnviada",
                table: "EVENTO_CALENDARIO");

            migrationBuilder.DropColumn(
                name: "Notificar",
                table: "EVENTO_CALENDARIO");
        }
    }
}
