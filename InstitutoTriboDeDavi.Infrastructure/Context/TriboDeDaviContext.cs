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
            builder.ApplyConfiguration(new AulaMap());
            builder.ApplyConfiguration(new PresencaMap());
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
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Polo> Polos { get; set; }
        public DbSet<Aula> Aulas { get; set; }
        public DbSet<Presenca> Presencas { get; set; }
        public DbSet<SincronizacaoHistorico> SincronizacaoHistoricos { get; set; }
    }
}
