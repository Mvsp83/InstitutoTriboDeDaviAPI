using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Context
{
    public class TriboDeDaviContext : DbContext
    {
        public TriboDeDaviContext()
        {
        }

        public TriboDeDaviContext(DbContextOptions<TriboDeDaviContext> options) : base(options)
        {
        }

        // *** OnConfiguring REMOVIDO ***
        // A connection string vem do appsettings / variável de ambiente

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UsuarioMap());
            builder.ApplyConfiguration(new AlunoMap());
            builder.ApplyConfiguration(new PoloMap());
            builder.ApplyConfiguration(new HorarioTurmaMap());
            builder.ApplyConfiguration(new AulaMap());
            builder.ApplyConfiguration(new PresencaMap());
            builder.ApplyConfiguration(new PlanoDeAulaMap());
            builder.ApplyConfiguration(new BlocoDoPlanoMap());
            builder.ApplyConfiguration(new ModeloDeAulaMap());
            builder.ApplyConfiguration(new BlocoDoModeloMap());
            builder.ApplyConfiguration(new AtividadeMap());
            builder.ApplyConfiguration(new AtividadeDoBlocoMap());
            builder.ApplyConfiguration(new RelatorioSalvoMap());
            builder.ApplyConfiguration(new ConfiguracaoDocumentoMap());
            builder.ApplyConfiguration(new ConfiguracaoDashboardMap());
            builder.ApplyConfiguration(new EventoCalendarioMap());
            builder.ApplyConfiguration(new DocumentoOficialMap());
            builder.ApplyConfiguration(new BemPatrimonialMap());
            builder.ApplyConfiguration(new AvisoMap());
            builder.ApplyConfiguration(new AvisoCienteMap());
            builder.ApplyConfiguration(new SolicitacaoInternaMap());
            builder.ApplyConfiguration(new MensagemSolicitacaoMap());
            builder.ApplyConfiguration(new ProdutoMap());
            builder.ApplyConfiguration(new VariacaoProdutoMap());
            builder.ApplyConfiguration(new AtletaMap());
            builder.ApplyConfiguration(new AvaliacaoFisicaMap());
            builder.ApplyConfiguration(new IndicadorAvaliacaoMap());
            builder.ApplyConfiguration(new CompeticaoMap());
            builder.ApplyConfiguration(new AnotacaoAtletaMap());
            builder.ApplyConfiguration(new MetaAtletaMap());
            builder.ApplyConfiguration(new LesaoMap());
            builder.ApplyConfiguration(new ContaFinanceiraMap());
            builder.ApplyConfiguration(new MovimentacaoFinanceiraMap());
            builder.ApplyConfiguration(new InscricaoMap());
            builder.ApplyConfiguration(new MatriculaMap());
            builder.ApplyConfiguration(new GraduacaoMap());
            builder.ApplyConfiguration(new DoadorMap());
            builder.ApplyConfiguration(new DoacaoMap());
            builder.ApplyConfiguration(new LogAuditoriaMap());
            builder.ApplyConfiguration(new RefreshTokenMap());
            builder.ApplyConfiguration(new PushSubscriptionMap());
            builder.ApplyConfiguration(new DocumentoArquivoMap());
            builder.ApplyConfiguration(new OcorrenciaAlunoMap());
            builder.ApplyConfiguration(new PlanoMensalidadeMap());
            builder.ApplyConfiguration(new MatriculaFinanceiraMap());
            builder.ApplyConfiguration(new CobrancaMap());
            builder.ApplyConfiguration(new FotoTreinoMap());
            builder.ApplyConfiguration(new FotoArquivoMap());
            builder.ApplyConfiguration(new PoloFotoConfigMap());
            builder.ApplyConfiguration(new ConfiguracaoFotoAlunoMap());
            builder.ApplyConfiguration(new VideoGaleriaMap());
            builder.Entity<SincronizacaoHistorico>(entity =>
            {
                entity.ToTable("SINCRONIZACAO_HISTORICO");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PoloNome).HasMaxLength(120);
                entity.Property(e => e.Origem).HasMaxLength(20);
                entity.Property(e => e.Erros).IsRequired(false); // string → nvarchar(max) no SQL Server
            });
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Polo> Polos { get; set; }
        public DbSet<HorarioTurma> HorariosTurma { get; set; }
        public DbSet<Aula> Aulas { get; set; }
        public DbSet<Presenca> Presencas { get; set; }
        public DbSet<SincronizacaoHistorico> SincronizacaoHistoricos { get; set; }
        public DbSet<PlanoDeAula> PlanosDeAula { get; set; }
        public DbSet<BlocoDoPlano> BlocosDoPlano { get; set; }
        public DbSet<ModeloDeAula> ModelosDeAula { get; set; }
        public DbSet<BlocoDoModelo> BlocosDoModelo { get; set; }
        public DbSet<Atividade> Atividades { get; set; }
        public DbSet<AtividadeDoBloco> AtividadesDoBloco { get; set; }
        public DbSet<RelatorioSalvo> RelatoriosSalvos { get; set; }
        public DbSet<ConfiguracaoDocumento> ConfiguracoesDocumento { get; set; }
        public DbSet<ConfiguracaoDashboard> ConfiguracoesDashboard { get; set; }
        public DbSet<EventoCalendario> EventosCalendario { get; set; }
        public DbSet<DocumentoOficial> DocumentosOficiais { get; set; }
        public DbSet<BemPatrimonial> BensPatrimoniais { get; set; }
        public DbSet<Aviso> Avisos { get; set; }
        public DbSet<AvisoCiente> AvisosCientes { get; set; }
        public DbSet<SolicitacaoInterna> SolicitacoesInternas { get; set; }
        public DbSet<MensagemSolicitacao> MensagensSolicitacao { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<VariacaoProduto> VariacoesProduto { get; set; }
        public DbSet<Atleta> Atletas { get; set; }
        public DbSet<AvaliacaoFisica> AvaliacoesFisicas { get; set; }
        public DbSet<Competicao> Competicoes { get; set; }
        public DbSet<AnotacaoAtleta> AnotacoesAtleta { get; set; }
        public DbSet<MetaAtleta> MetasAtleta { get; set; }
        public DbSet<Lesao> Lesoes { get; set; }
        public DbSet<ContaFinanceira> ContasFinanceiras { get; set; }
        public DbSet<MovimentacaoFinanceira> MovimentacoesFinanceiras { get; set; }
        public DbSet<Inscricao> Inscricoes { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Graduacao> Graduacoes { get; set; }
        public DbSet<Doador> Doadores { get; set; }
        public DbSet<Doacao> Doacoes { get; set; }
        public DbSet<LogAuditoria> LogsAuditoria { get; set; }
        public DbSet<PushSubscription> PushSubscriptions { get; set; }
        public DbSet<DocumentoArquivo> DocumentosArquivo { get; set; }
        public DbSet<OcorrenciaAluno> OcorrenciasAluno { get; set; }
        public DbSet<PlanoMensalidade> PlanosMensalidade { get; set; }
        public DbSet<MatriculaFinanceira> MatriculasFinanceiras { get; set; }
        public DbSet<Cobranca> Cobrancas { get; set; }
        public DbSet<FotoTreino> FotosTreino { get; set; }
        public DbSet<FotoArquivo> FotosArquivo { get; set; }
        public DbSet<PoloFotoConfig> PoloFotoConfigs { get; set; }
        public DbSet<ConfiguracaoFotoAluno> ConfiguracoesFotoAluno { get; set; }
        public DbSet<VideoGaleria> VideosGaleria { get; set; }
    }
}
