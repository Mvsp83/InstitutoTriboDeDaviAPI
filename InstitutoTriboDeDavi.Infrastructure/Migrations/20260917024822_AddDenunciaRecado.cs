using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDenunciaRecado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DENUNCIA_RECADO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecadoId = table.Column<long>(type: "bigint", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DenunciadoPor = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Resolvida = table.Column<bool>(type: "boolean", nullable: false),
                    ResolvidoPor = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    DataResolucao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DENUNCIA_RECADO", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DENUNCIA_RECADO_RecadoId",
                table: "DENUNCIA_RECADO",
                column: "RecadoId");

            migrationBuilder.CreateIndex(
                name: "IX_DENUNCIA_RECADO_Resolvida",
                table: "DENUNCIA_RECADO",
                column: "Resolvida");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DENUNCIA_RECADO");
        }
    }
}
