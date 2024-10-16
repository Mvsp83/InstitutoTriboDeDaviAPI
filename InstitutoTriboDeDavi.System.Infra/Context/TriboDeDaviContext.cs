using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Domain.Entities.Business;
using InstitutoTriboDeDavi.System.Infra.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InstitutoTriboDeDavi.System.Infra.Context
{
    public class TriboDeDaviContext : DbContext
    {
        public TriboDeDaviContext()
        {

        }

        public TriboDeDaviContext(DbContextOptions<TriboDeDaviContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("");
                
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UsuarioMap());
            builder.ApplyConfiguration(new AlunoMap());
            builder.ApplyConfiguration(new PoloMap());
            builder.ApplyConfiguration(new PaisMap());
            builder.ApplyConfiguration(new EstadoMap());
            builder.ApplyConfiguration(new CidadeMap());
            builder.ApplyConfiguration(new BairroMap());
            builder.ApplyConfiguration(new EnderecoMap());
            builder.ApplyConfiguration(new ResponsavelMap());
            builder.ApplyConfiguration(new AulaMap());
            builder.ApplyConfiguration(new PresencaMap());
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Polo> Polos { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Cidade> Cidades { get; set; }
        public DbSet<Bairro> Bairros { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Aula> Aulas { get; set; }
        public DbSet<Presenca> Presencas { get; set; }
        public DbSet<Responsavel> Responsaveis { get; set; }

    }
}
