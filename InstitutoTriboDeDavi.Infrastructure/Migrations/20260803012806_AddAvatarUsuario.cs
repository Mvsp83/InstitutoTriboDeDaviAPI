using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "USUARIOS",
                type: "VARCHAR(MAX)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "USUARIOS");
        }
    }
}
