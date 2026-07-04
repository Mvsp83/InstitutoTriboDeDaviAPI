using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ALUNOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "VARCHAR(120)", maxLength: 120, nullable: false),
                    RG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CPF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Peso = table.Column<double>(type: "float", nullable: true),
                    Faixa = table.Column<int>(type: "int", nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Celular = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Responsavel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parentesco = table.Column<int>(type: "int", nullable: true),
                    RGResponsavel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CPFResponsavel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Escola = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Periodo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Turma = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALUNOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AULAS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFim = table.Column<TimeSpan>(type: "time", nullable: false),
                    PresencaSalva = table.Column<bool>(type: "bit", nullable: false),
                    Turma = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AULAS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "POLOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "VARCHAR(120)", maxLength: 120, nullable: false),
                    Informacoes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POLOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PRESENCAS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    NomeAluno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstaPresente = table.Column<bool>(type: "bit", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AulaId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRESENCAS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "VARCHAR(180)", nullable: false),
                    Login = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "VARCHAR(200)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: true),
                    PoloNome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALUNOS");

            migrationBuilder.DropTable(
                name: "AULAS");

            migrationBuilder.DropTable(
                name: "POLOS");

            migrationBuilder.DropTable(
                name: "PRESENCAS");

            migrationBuilder.DropTable(
                name: "USUARIOS");
        }
    }
}
