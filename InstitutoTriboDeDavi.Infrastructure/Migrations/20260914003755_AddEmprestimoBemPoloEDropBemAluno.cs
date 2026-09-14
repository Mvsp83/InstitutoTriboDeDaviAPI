using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmprestimoBemPoloEDropBemAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlunoId",
                table: "BEM_PATRIMONIAL");

            migrationBuilder.AlterColumn<long>(
                name: "AlunoId",
                table: "EMPRESTIMO_BEM",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "PoloId",
                table: "EMPRESTIMO_BEM",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EMPRESTIMO_BEM_AlunoId",
                table: "EMPRESTIMO_BEM",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_EMPRESTIMO_BEM_PoloId",
                table: "EMPRESTIMO_BEM",
                column: "PoloId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EMPRESTIMO_BEM_AlunoId",
                table: "EMPRESTIMO_BEM");

            migrationBuilder.DropIndex(
                name: "IX_EMPRESTIMO_BEM_PoloId",
                table: "EMPRESTIMO_BEM");

            migrationBuilder.DropColumn(
                name: "PoloId",
                table: "EMPRESTIMO_BEM");

            migrationBuilder.AlterColumn<long>(
                name: "AlunoId",
                table: "EMPRESTIMO_BEM",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AlunoId",
                table: "BEM_PATRIMONIAL",
                type: "bigint",
                nullable: true);
        }
    }
}
