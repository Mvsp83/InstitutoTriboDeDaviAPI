using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Infra.Mappings;
using Microsoft.EntityFrameworkCore;

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

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("");
        //}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UsuarioMap());
        }

        public DbSet<Usuario> Users { get; set; }
    }
}
