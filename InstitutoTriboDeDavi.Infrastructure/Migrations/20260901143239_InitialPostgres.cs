using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InstitutoTriboDeDavi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ALUNOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "VARCHAR(120)", maxLength: 120, nullable: false),
                    RG = table.Column<string>(type: "text", nullable: true),
                    CPF = table.Column<string>(type: "text", nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Peso = table.Column<double>(type: "double precision", nullable: true),
                    Altura = table.Column<double>(type: "double precision", nullable: true),
                    Faixa = table.Column<int>(type: "integer", nullable: false),
                    Endereco = table.Column<string>(type: "text", nullable: true),
                    Numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Complemento = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Celular = table.Column<string>(type: "text", nullable: true),
                    Telefone2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Responsavel = table.Column<string>(type: "text", nullable: true),
                    Parentesco = table.Column<int>(type: "integer", nullable: true),
                    RGResponsavel = table.Column<string>(type: "text", nullable: true),
                    CPFResponsavel = table.Column<string>(type: "text", nullable: true),
                    Escola = table.Column<string>(type: "text", nullable: true),
                    Serie = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Periodo = table.Column<string>(type: "text", nullable: true),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Turma = table.Column<int>(type: "integer", nullable: false),
                    AnonimizadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CodigoResponsavel = table.Column<string>(type: "VARCHAR(16)", maxLength: 16, nullable: true),
                    AutorizaImagem = table.Column<bool>(type: "boolean", nullable: true),
                    AutorizaImagemEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EhAdulto = table.Column<bool>(type: "boolean", nullable: false),
                    FotoArquivoId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALUNOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ATIVIDADES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Principio = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReferenciaBiblica = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    VideoUrl = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATIVIDADES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ATLETA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    CategoriaPeso = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Objetivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AULAS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraFim = table.Column<TimeSpan>(type: "interval", nullable: false),
                    PresencaSalva = table.Column<bool>(type: "boolean", nullable: false),
                    Turma = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AULAS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AVISO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Mensagem = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    PublicoAlvo = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AVISO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AVISO_CIENTE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AvisoId = table.Column<long>(type: "bigint", nullable: false),
                    UsuarioLogin = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    DataCiente = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AVISO_CIENTE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BEM_PATRIMONIAL",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DataAquisicao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: true),
                    NumeroPatrimonio = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BEM_PATRIMONIAL", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "COBRANCA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    PlanoId = table.Column<long>(type: "bigint", nullable: true),
                    Competencia = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Vencimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PagamentoData = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PagamentoValor = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PagamentoForma = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    ContaId = table.Column<long>(type: "bigint", nullable: true),
                    MovimentacaoId = table.Column<long>(type: "bigint", nullable: true),
                    Observacao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COBRANCA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "COMPETICAO_EVENTO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataFim = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Local = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Organizador = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    PrazoInscricao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Link = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Observacao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMPETICAO_EVENTO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CONFIG_FOTO_ALUNO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MostrarNoCadastro = table.Column<bool>(type: "boolean", nullable: false),
                    MostrarNaChamada = table.Column<bool>(type: "boolean", nullable: false),
                    MostrarNoResponsavel = table.Column<bool>(type: "boolean", nullable: false),
                    MostrarNaCarteirinha = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONFIG_FOTO_ALUNO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CONFIGURACAO_DASHBOARD",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioLogin = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Layout = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONFIGURACAO_DASHBOARD", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CONFIGURACAO_DOCUMENTO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TituloCabecalho = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    LinhaExtra = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    TextoRodape = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    MostrarLogo = table.Column<bool>(type: "boolean", nullable: false),
                    MostrarDataGeracao = table.Column<bool>(type: "boolean", nullable: false),
                    TextosPadraoJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONFIGURACAO_DOCUMENTO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CONTA_FINANCEIRA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Banco = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Agencia = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Numero = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    SaldoInicial = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTA_FINANCEIRA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DOACAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DoadorId = table.Column<long>(type: "bigint", nullable: true),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Forma = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Finalidade = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ReciboDocumentoId = table.Column<long>(type: "bigint", nullable: true),
                    ReciboNumero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    RegistradoPor = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOACAO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DOADOR",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoPessoa = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Documento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    Telefone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Endereco = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Cidade = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOADOR", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DOCUMENTO_ARQUIVO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    Conteudo = table.Column<byte[]>(type: "bytea", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCUMENTO_ARQUIVO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DOCUMENTO_OFICIAL",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    NumeroFormatado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DataDocumento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Conteudo = table.Column<string>(type: "text", nullable: true),
                    DataAprovacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCUMENTO_OFICIAL", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EVENTO_CALENDARIO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataFim = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PoloId = table.Column<long>(type: "bigint", nullable: true),
                    Interno = table.Column<bool>(type: "boolean", nullable: false),
                    Notificar = table.Column<bool>(type: "boolean", nullable: false),
                    EmailsNotificacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DiasAntecedencia = table.Column<int>(type: "integer", nullable: false),
                    NotificacaoEnviada = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENTO_CALENDARIO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FOTO_ARQUIVO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Conteudo = table.Column<byte[]>(type: "bytea", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FOTO_ARQUIVO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FOTO_TREINO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Categoria = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Turma = table.Column<int>(type: "integer", nullable: false),
                    DataAula = table.Column<DateTime>(type: "date", nullable: false),
                    Legenda = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ArquivoId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProfessorId = table.Column<long>(type: "bigint", nullable: false),
                    Publicada = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ConsentimentoConfirmado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FOTO_TREINO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GRADUACAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    FaixaAnterior = table.Column<int>(type: "integer", nullable: false),
                    FaixaNova = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Observacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RegistradoPor = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRADUACAO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "INSCRICAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Publico = table.Column<int>(type: "integer", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Turma = table.Column<int>(type: "integer", nullable: true),
                    JaEraAluno = table.Column<bool>(type: "boolean", nullable: false),
                    TurmaAnterior = table.Column<int>(type: "integer", nullable: true),
                    Nome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Rg = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Cpf = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Peso = table.Column<decimal>(type: "numeric(6,2)", nullable: true),
                    Altura = table.Column<decimal>(type: "numeric(4,2)", nullable: true),
                    Faixa = table.Column<int>(type: "integer", nullable: false),
                    Escola = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    Serie = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Periodo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Parentesco = table.Column<int>(type: "integer", nullable: false),
                    ParentescoOutro = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    NomeResponsavel = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    RgResponsavel = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CpfResponsavel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Rua = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Complemento = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Bairro = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Cidade = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    WhatsApp = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Telefone2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    RespostasSaudeJson = table.Column<string>(type: "text", nullable: true),
                    RespostasFamiliarJson = table.Column<string>(type: "text", nullable: true),
                    TemRestricaoMedica = table.Column<bool>(type: "boolean", nullable: false),
                    Medicamentos = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AceitouTermo = table.Column<bool>(type: "boolean", nullable: false),
                    AceitouImagem = table.Column<bool>(type: "boolean", nullable: false),
                    AceitouComodato = table.Column<bool>(type: "boolean", nullable: false),
                    AceitouLgpd = table.Column<bool>(type: "boolean", nullable: false),
                    NomeAssinatura = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    VersaoTermos = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DataEnvio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FotoArquivoId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CodigoResponsavel = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AlunoId = table.Column<long>(type: "bigint", nullable: true),
                    DataRevisao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RevisadoPor = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ObservacaoRevisao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Anonimizada = table.Column<bool>(type: "boolean", nullable: false),
                    AnonimizadaEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INSCRICAO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LOG_AUDITORIA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UsuarioLogin = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Acao = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Entidade = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    EntidadeId = table.Column<long>(type: "bigint", nullable: false),
                    Resumo = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Alteracoes = table.Column<string>(type: "text", nullable: true),
                    Ip = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOG_AUDITORIA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MATRICULA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Turma = table.Column<int>(type: "integer", nullable: false),
                    InscricaoId = table.Column<long>(type: "bigint", nullable: true),
                    DataMatricula = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false),
                    DataEncerramento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MotivoEncerramento = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MATRICULA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MATRICULA_FINANCEIRA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    PlanoId = table.Column<long>(type: "bigint", nullable: false),
                    DiaVencimento = table.Column<int>(type: "integer", nullable: false),
                    Inicio = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DescontoTipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DescontoValor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Observacao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MATRICULA_FINANCEIRA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MODELOS_DE_AULA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DuracaoTotalMinutos = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MODELOS_DE_AULA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MOVIMENTACAO_FINANCEIRA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContaId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CategoriaId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Conciliado = table.Column<bool>(type: "boolean", nullable: false),
                    Documento = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TransferenciaId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOVIMENTACAO_FINANCEIRA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OCORRENCIA_ALUNO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Texto = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RegistradoPor = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OCORRENCIA_ALUNO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PLANO_MENSALIDADE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    OpcoesVencimento = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PLANO_MENSALIDADE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PLANOS_DE_AULA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Turma = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Objetivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DataPrevista = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DuracaoTotalMinutos = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AulaId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PLANOS_DE_AULA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "POLO_FOTO_CONFIG",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    RequerAutorizacao = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POLO_FOTO_CONFIG", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "POLOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "VARCHAR(120)", maxLength: 120, nullable: false),
                    Informacoes = table.Column<string>(type: "text", nullable: false),
                    Endereco = table.Column<string>(type: "text", nullable: false),
                    Bairro = table.Column<string>(type: "text", nullable: false),
                    Cidade = table.Column<string>(type: "text", nullable: false),
                    LimiteAlunos = table.Column<int>(type: "integer", nullable: false)
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlunoId = table.Column<long>(type: "bigint", nullable: false),
                    NomeAluno = table.Column<string>(type: "text", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EstaPresente = table.Column<bool>(type: "boolean", nullable: false),
                    Observacoes = table.Column<string>(type: "text", nullable: false),
                    AulaId = table.Column<long>(type: "bigint", nullable: false),
                    JustificativaResponsavel = table.Column<string>(type: "text", nullable: true),
                    JustificadaEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRESENCAS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PRODUTO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Preco = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    FotoArquivoId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FormasPagamento = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Informacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUTO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PUSH_SUBSCRIPTION",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioLogin = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Endpoint = table.Column<string>(type: "text", nullable: false),
                    P256dh = table.Column<string>(type: "text", nullable: false),
                    Auth = table.Column<string>(type: "text", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PUSH_SUBSCRIPTION", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "REFRESH_TOKENS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<long>(type: "BIGINT", nullable: false),
                    TokenHash = table.Column<string>(type: "VARCHAR(128)", maxLength: 128, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExpiraEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RevogadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REFRESH_TOKENS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RELATORIOS_SALVOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioLogin = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    FonteId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Colunas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Turma = table.Column<int>(type: "integer", nullable: true),
                    PoloId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RELATORIOS_SALVOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SINCRONIZACAO_HISTORICO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataExecucao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PoloNome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: false),
                    Inseridos = table.Column<int>(type: "integer", nullable: false),
                    Atualizados = table.Column<int>(type: "integer", nullable: false),
                    Ignorados = table.Column<int>(type: "integer", nullable: false),
                    Sucesso = table.Column<bool>(type: "boolean", nullable: false),
                    Erros = table.Column<string>(type: "text", nullable: true),
                    Origem = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SINCRONIZACAO_HISTORICO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SOLICITACAO_INTERNA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Assunto = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: true),
                    PoloNome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    CriadoPorLogin = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    CriadoPorRole = table.Column<int>(type: "integer", nullable: false),
                    DestinatarioLogin = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOLICITACAO_INTERNA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "VARCHAR(180)", nullable: false),
                    Login = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "VARCHAR(200)", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    PoloId = table.Column<long>(type: "bigint", nullable: true),
                    PoloNome = table.Column<string>(type: "text", nullable: false),
                    PermiteGraduacao = table.Column<bool>(type: "boolean", nullable: false),
                    Avatar = table.Column<string>(type: "text", nullable: true),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    Faixa = table.Column<int>(type: "integer", nullable: true),
                    FotoSite = table.Column<string>(type: "text", nullable: true),
                    MostrarNoSite = table.Column<bool>(type: "boolean", nullable: false),
                    TotpSecret = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: true),
                    TotpConfirmado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VIDEO_GALERIA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    YoutubeId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Url = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VIDEO_GALERIA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ATLETA_ANOTACAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AtletaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Texto = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Autor = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA_ANOTACAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATLETA_ANOTACAO_ATLETA_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "ATLETA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ATLETA_AVALIACAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AtletaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Observacao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA_AVALIACAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATLETA_AVALIACAO_ATLETA_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "ATLETA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ATLETA_COMPETICAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AtletaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Evento = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CategoriaPeso = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Colocacao = table.Column<int>(type: "integer", nullable: false),
                    Lutas = table.Column<int>(type: "integer", nullable: false),
                    Vitorias = table.Column<int>(type: "integer", nullable: false),
                    Finalizacoes = table.Column<int>(type: "integer", nullable: false),
                    Observacao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA_COMPETICAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATLETA_COMPETICAO_ATLETA_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "ATLETA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ATLETA_LESAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AtletaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Local = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Gravidade = table.Column<int>(type: "integer", nullable: false),
                    DataRetorno = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Recuperado = table.Column<bool>(type: "boolean", nullable: false),
                    Observacao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA_LESAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATLETA_LESAO_ATLETA_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "ATLETA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ATLETA_META",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AtletaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Prazo = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA_META", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATLETA_META_ATLETA_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "ATLETA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "COMPETICAO_PARTICIPACAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompeticaoEventoId = table.Column<long>(type: "BIGINT", nullable: false),
                    AtletaId = table.Column<long>(type: "bigint", nullable: false),
                    CategoriaPeso = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Colocacao = table.Column<int>(type: "integer", nullable: false),
                    Lutas = table.Column<int>(type: "integer", nullable: false),
                    Vitorias = table.Column<int>(type: "integer", nullable: false),
                    Finalizacoes = table.Column<int>(type: "integer", nullable: false),
                    Observacao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMPETICAO_PARTICIPACAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_COMPETICAO_PARTICIPACAO_COMPETICAO_EVENTO_CompeticaoEventoId",
                        column: x => x.CompeticaoEventoId,
                        principalTable: "COMPETICAO_EVENTO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BLOCOS_DO_MODELO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModeloDeAulaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    DuracaoMinutos = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BLOCOS_DO_MODELO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BLOCOS_DO_MODELO_MODELOS_DE_AULA_ModeloDeAulaId",
                        column: x => x.ModeloDeAulaId,
                        principalTable: "MODELOS_DE_AULA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BLOCOS_DO_PLANO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlanoDeAulaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    DuracaoMinutos = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BLOCOS_DO_PLANO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BLOCOS_DO_PLANO_PLANOS_DE_AULA_PlanoDeAulaId",
                        column: x => x.PlanoDeAulaId,
                        principalTable: "PLANOS_DE_AULA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "POLO_HORARIO_TURMA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PoloId = table.Column<long>(type: "BIGINT", nullable: false),
                    Turma = table.Column<int>(type: "integer", nullable: false),
                    DiaSemana = table.Column<int>(type: "integer", nullable: false),
                    HoraInicio = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    HoraFim = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POLO_HORARIO_TURMA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_POLO_HORARIO_TURMA_POLOS_PoloId",
                        column: x => x.PoloId,
                        principalTable: "POLOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PRODUTO_VARIACAO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProdutoId = table.Column<long>(type: "BIGINT", nullable: false),
                    Tamanho = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Cor = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Quantidade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUTO_VARIACAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PRODUTO_VARIACAO_PRODUTO_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "PRODUTO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SOLICITACAO_MENSAGEM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SolicitacaoInternaId = table.Column<long>(type: "BIGINT", nullable: false),
                    AutorLogin = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    AutorRole = table.Column<int>(type: "integer", nullable: false),
                    Texto = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOLICITACAO_MENSAGEM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SOLICITACAO_MENSAGEM_SOLICITACAO_INTERNA_SolicitacaoInterna~",
                        column: x => x.SolicitacaoInternaId,
                        principalTable: "SOLICITACAO_INTERNA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ATLETA_AVALIACAO_INDICADOR",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AvaliacaoFisicaId = table.Column<long>(type: "BIGINT", nullable: false),
                    Nome = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Unidade = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATLETA_AVALIACAO_INDICADOR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATLETA_AVALIACAO_INDICADOR_ATLETA_AVALIACAO_AvaliacaoFisica~",
                        column: x => x.AvaliacaoFisicaId,
                        principalTable: "ATLETA_AVALIACAO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ATIVIDADES_DO_BLOCO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                name: "IX_ALUNOS_CodigoResponsavel",
                table: "ALUNOS",
                column: "CodigoResponsavel");

            migrationBuilder.CreateIndex(
                name: "IX_ATIVIDADES_DO_BLOCO_AtividadeId",
                table: "ATIVIDADES_DO_BLOCO",
                column: "AtividadeId");

            migrationBuilder.CreateIndex(
                name: "IX_ATIVIDADES_DO_BLOCO_BlocoDoPlanoId",
                table: "ATIVIDADES_DO_BLOCO",
                column: "BlocoDoPlanoId");

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_AlunoId",
                table: "ATLETA",
                column: "AlunoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_ANOTACAO_AtletaId",
                table: "ATLETA_ANOTACAO",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_AVALIACAO_AtletaId",
                table: "ATLETA_AVALIACAO",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_AVALIACAO_INDICADOR_AvaliacaoFisicaId",
                table: "ATLETA_AVALIACAO_INDICADOR",
                column: "AvaliacaoFisicaId");

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_COMPETICAO_AtletaId",
                table: "ATLETA_COMPETICAO",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_LESAO_AtletaId",
                table: "ATLETA_LESAO",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_ATLETA_META_AtletaId",
                table: "ATLETA_META",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_AVISO_CIENTE_AvisoId_UsuarioLogin",
                table: "AVISO_CIENTE",
                columns: new[] { "AvisoId", "UsuarioLogin" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BLOCOS_DO_MODELO_ModeloDeAulaId",
                table: "BLOCOS_DO_MODELO",
                column: "ModeloDeAulaId");

            migrationBuilder.CreateIndex(
                name: "IX_BLOCOS_DO_PLANO_PlanoDeAulaId",
                table: "BLOCOS_DO_PLANO",
                column: "PlanoDeAulaId");

            migrationBuilder.CreateIndex(
                name: "IX_COBRANCA_AlunoId_Competencia",
                table: "COBRANCA",
                columns: new[] { "AlunoId", "Competencia" });

            migrationBuilder.CreateIndex(
                name: "IX_COBRANCA_Competencia",
                table: "COBRANCA",
                column: "Competencia");

            migrationBuilder.CreateIndex(
                name: "IX_COMPETICAO_PARTICIPACAO_AtletaId",
                table: "COMPETICAO_PARTICIPACAO",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_COMPETICAO_PARTICIPACAO_CompeticaoEventoId",
                table: "COMPETICAO_PARTICIPACAO",
                column: "CompeticaoEventoId");

            migrationBuilder.CreateIndex(
                name: "IX_CONFIGURACAO_DASHBOARD_UsuarioLogin",
                table: "CONFIGURACAO_DASHBOARD",
                column: "UsuarioLogin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOACAO_Data",
                table: "DOACAO",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_DOACAO_DoadorId",
                table: "DOACAO",
                column: "DoadorId");

            migrationBuilder.CreateIndex(
                name: "IX_DOADOR_Nome",
                table: "DOADOR",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENTO_ARQUIVO_Categoria",
                table: "DOCUMENTO_ARQUIVO",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENTO_OFICIAL_Tipo_Ano_Numero",
                table: "DOCUMENTO_OFICIAL",
                columns: new[] { "Tipo", "Ano", "Numero" },
                unique: true,
                filter: "\"Status\" = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FOTO_TREINO_Categoria",
                table: "FOTO_TREINO",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_FOTO_TREINO_PoloId_Turma_DataAula",
                table: "FOTO_TREINO",
                columns: new[] { "PoloId", "Turma", "DataAula" },
                unique: true,
                filter: "\"Categoria\" = 'polo'");

            migrationBuilder.CreateIndex(
                name: "IX_FOTO_TREINO_Publicada",
                table: "FOTO_TREINO",
                column: "Publicada");

            migrationBuilder.CreateIndex(
                name: "IX_GRADUACAO_AlunoId",
                table: "GRADUACAO",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_GRADUACAO_Data_PoloId",
                table: "GRADUACAO",
                columns: new[] { "Data", "PoloId" });

            migrationBuilder.CreateIndex(
                name: "IX_INSCRICAO_PoloId",
                table: "INSCRICAO",
                column: "PoloId");

            migrationBuilder.CreateIndex(
                name: "IX_INSCRICAO_Status_Ano",
                table: "INSCRICAO",
                columns: new[] { "Status", "Ano" });

            migrationBuilder.CreateIndex(
                name: "IX_LOG_AUDITORIA_Data",
                table: "LOG_AUDITORIA",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_LOG_AUDITORIA_Entidade_EntidadeId",
                table: "LOG_AUDITORIA",
                columns: new[] { "Entidade", "EntidadeId" });

            migrationBuilder.CreateIndex(
                name: "IX_MATRICULA_AlunoId_Ano",
                table: "MATRICULA",
                columns: new[] { "AlunoId", "Ano" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MATRICULA_Ano_PoloId",
                table: "MATRICULA",
                columns: new[] { "Ano", "PoloId" });

            migrationBuilder.CreateIndex(
                name: "IX_MATRICULA_FINANCEIRA_AlunoId",
                table: "MATRICULA_FINANCEIRA",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_MATRICULA_FINANCEIRA_PlanoId",
                table: "MATRICULA_FINANCEIRA",
                column: "PlanoId");

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTACAO_FINANCEIRA_ContaId",
                table: "MOVIMENTACAO_FINANCEIRA",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTACAO_FINANCEIRA_Data",
                table: "MOVIMENTACAO_FINANCEIRA",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTACAO_FINANCEIRA_TransferenciaId",
                table: "MOVIMENTACAO_FINANCEIRA",
                column: "TransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_OCORRENCIA_ALUNO_AlunoId",
                table: "OCORRENCIA_ALUNO",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_PLANOS_DE_AULA_PoloId_Turma_DataPrevista",
                table: "PLANOS_DE_AULA",
                columns: new[] { "PoloId", "Turma", "DataPrevista" });

            migrationBuilder.CreateIndex(
                name: "IX_POLO_FOTO_CONFIG_PoloId",
                table: "POLO_FOTO_CONFIG",
                column: "PoloId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_POLO_HORARIO_TURMA_PoloId",
                table: "POLO_HORARIO_TURMA",
                column: "PoloId");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUTO_VARIACAO_ProdutoId",
                table: "PRODUTO_VARIACAO",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_PUSH_SUBSCRIPTION_UsuarioLogin",
                table: "PUSH_SUBSCRIPTION",
                column: "UsuarioLogin");

            migrationBuilder.CreateIndex(
                name: "IX_REFRESH_TOKENS_TokenHash",
                table: "REFRESH_TOKENS",
                column: "TokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_REFRESH_TOKENS_UsuarioId",
                table: "REFRESH_TOKENS",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RELATORIOS_SALVOS_UsuarioLogin",
                table: "RELATORIOS_SALVOS",
                column: "UsuarioLogin");

            migrationBuilder.CreateIndex(
                name: "IX_SOLICITACAO_INTERNA_CriadoPorLogin",
                table: "SOLICITACAO_INTERNA",
                column: "CriadoPorLogin");

            migrationBuilder.CreateIndex(
                name: "IX_SOLICITACAO_INTERNA_DestinatarioLogin",
                table: "SOLICITACAO_INTERNA",
                column: "DestinatarioLogin");

            migrationBuilder.CreateIndex(
                name: "IX_SOLICITACAO_MENSAGEM_SolicitacaoInternaId",
                table: "SOLICITACAO_MENSAGEM",
                column: "SolicitacaoInternaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALUNOS");

            migrationBuilder.DropTable(
                name: "ATIVIDADES_DO_BLOCO");

            migrationBuilder.DropTable(
                name: "ATLETA_ANOTACAO");

            migrationBuilder.DropTable(
                name: "ATLETA_AVALIACAO_INDICADOR");

            migrationBuilder.DropTable(
                name: "ATLETA_COMPETICAO");

            migrationBuilder.DropTable(
                name: "ATLETA_LESAO");

            migrationBuilder.DropTable(
                name: "ATLETA_META");

            migrationBuilder.DropTable(
                name: "AULAS");

            migrationBuilder.DropTable(
                name: "AVISO");

            migrationBuilder.DropTable(
                name: "AVISO_CIENTE");

            migrationBuilder.DropTable(
                name: "BEM_PATRIMONIAL");

            migrationBuilder.DropTable(
                name: "BLOCOS_DO_MODELO");

            migrationBuilder.DropTable(
                name: "COBRANCA");

            migrationBuilder.DropTable(
                name: "COMPETICAO_PARTICIPACAO");

            migrationBuilder.DropTable(
                name: "CONFIG_FOTO_ALUNO");

            migrationBuilder.DropTable(
                name: "CONFIGURACAO_DASHBOARD");

            migrationBuilder.DropTable(
                name: "CONFIGURACAO_DOCUMENTO");

            migrationBuilder.DropTable(
                name: "CONTA_FINANCEIRA");

            migrationBuilder.DropTable(
                name: "DOACAO");

            migrationBuilder.DropTable(
                name: "DOADOR");

            migrationBuilder.DropTable(
                name: "DOCUMENTO_ARQUIVO");

            migrationBuilder.DropTable(
                name: "DOCUMENTO_OFICIAL");

            migrationBuilder.DropTable(
                name: "EVENTO_CALENDARIO");

            migrationBuilder.DropTable(
                name: "FOTO_ARQUIVO");

            migrationBuilder.DropTable(
                name: "FOTO_TREINO");

            migrationBuilder.DropTable(
                name: "GRADUACAO");

            migrationBuilder.DropTable(
                name: "INSCRICAO");

            migrationBuilder.DropTable(
                name: "LOG_AUDITORIA");

            migrationBuilder.DropTable(
                name: "MATRICULA");

            migrationBuilder.DropTable(
                name: "MATRICULA_FINANCEIRA");

            migrationBuilder.DropTable(
                name: "MOVIMENTACAO_FINANCEIRA");

            migrationBuilder.DropTable(
                name: "OCORRENCIA_ALUNO");

            migrationBuilder.DropTable(
                name: "PLANO_MENSALIDADE");

            migrationBuilder.DropTable(
                name: "POLO_FOTO_CONFIG");

            migrationBuilder.DropTable(
                name: "POLO_HORARIO_TURMA");

            migrationBuilder.DropTable(
                name: "PRESENCAS");

            migrationBuilder.DropTable(
                name: "PRODUTO_VARIACAO");

            migrationBuilder.DropTable(
                name: "PUSH_SUBSCRIPTION");

            migrationBuilder.DropTable(
                name: "REFRESH_TOKENS");

            migrationBuilder.DropTable(
                name: "RELATORIOS_SALVOS");

            migrationBuilder.DropTable(
                name: "SINCRONIZACAO_HISTORICO");

            migrationBuilder.DropTable(
                name: "SOLICITACAO_MENSAGEM");

            migrationBuilder.DropTable(
                name: "USUARIOS");

            migrationBuilder.DropTable(
                name: "VIDEO_GALERIA");

            migrationBuilder.DropTable(
                name: "ATIVIDADES");

            migrationBuilder.DropTable(
                name: "BLOCOS_DO_PLANO");

            migrationBuilder.DropTable(
                name: "ATLETA_AVALIACAO");

            migrationBuilder.DropTable(
                name: "MODELOS_DE_AULA");

            migrationBuilder.DropTable(
                name: "COMPETICAO_EVENTO");

            migrationBuilder.DropTable(
                name: "POLOS");

            migrationBuilder.DropTable(
                name: "PRODUTO");

            migrationBuilder.DropTable(
                name: "SOLICITACAO_INTERNA");

            migrationBuilder.DropTable(
                name: "PLANOS_DE_AULA");

            migrationBuilder.DropTable(
                name: "ATLETA");
        }
    }
}
