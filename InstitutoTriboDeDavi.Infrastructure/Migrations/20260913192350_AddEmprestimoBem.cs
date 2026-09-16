using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmprestimoBem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EMPRESTIMO_BEM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BemPatrimonialId = table.Column<long>(type: "bigint", nullable: false),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    DataEmprestimo = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataDevolucao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Observacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RegistradoPor = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPRESTIMO_BEM", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EMPRESTIMO_BEM_BemPatrimonialId",
                table: "EMPRESTIMO_BEM",
                column: "BemPatrimonialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EMPRESTIMO_BEM");
        }
    }
}
