using AutoMapper;
using InstitutoTriboDeDavi.API.ViewModels.Create;
using InstitutoTriboDeDavi.API.ViewModels.Usuario;
using InstitutoTriboDeDavi.System.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.Infra.Context;
using InstitutoTriboDeDavi.System.Services;
using InstitutoTriboDeDavi.System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.API
{
    public class Startup : IStartup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            #region Injeção de Dependência

            services.AddSingleton(d => Configuration);
            services.AddDbContext<TriboDeDaviContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:TRIBODEDAVIAPI"]), ServiceLifetime.Transient);

            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IAlunoRepository, AlunoRepository>();
            services.AddScoped<IAlunoService, AlunoService>();
            services.AddScoped<IPoloRepository, PoloRepository>();
            services.AddScoped<IPoloService, PoloService>();
            services.AddScoped<IResponsavelRepository, ResponsavelRepository>();
            services.AddScoped<IResponsavelService, ResponsavelService>();
            services.AddScoped<IPaisRepository, PaisRepository>();
            services.AddScoped<IPaisService, PaisService>();
            services.AddScoped<IEstadoRepository, EstadoRepository>();
            services.AddScoped<IEstadoService, EstadoService>();
            services.AddScoped<ICidadeRepository, CidadeRepository>();
            services.AddScoped<ICidadeService, CidadeService>();
            services.AddScoped<IBairroRepository, BairroRepository>();
            services.AddScoped<IBairroService, BairroService>();
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            services.AddScoped<IEnderecoService, EnderecoService>();


            //services.AddTransient<FactoryPlanilhaDB>(provider =>
            //new FactoryPlanilhaDB(Configuration.GetConnectionString("ConnectionStrings:TRIBODEDAVIAPI")));


            #endregion

            #region AutoMapper

            var autoMapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Usuario, UsuarioDTO>().ReverseMap();
                cfg.CreateMap<UsuarioViewModel, UsuarioDTO>().ReverseMap();
                cfg.CreateMap<Aluno, AlunoDTO>().ReverseMap();
                cfg.CreateMap<AlunoViewModel, AlunoDTO>().ReverseMap();
                cfg.CreateMap<Polo, PoloDTO>().ReverseMap();
                cfg.CreateMap<PoloViewModel, PoloDTO>().ReverseMap();
                cfg.CreateMap<Pais, PaisDTO>().ReverseMap();
                cfg.CreateMap<PaisViewModel, PaisDTO>().ReverseMap();
                cfg.CreateMap<Estado, EstadoDTO>().ReverseMap();
                cfg.CreateMap<EstadoViewModel, EstadoDTO>().ReverseMap();
                cfg.CreateMap<Cidade, CidadeDTO>().ReverseMap();
                cfg.CreateMap<CidadeViewModel,  CidadeDTO>().ReverseMap();
                cfg.CreateMap<Bairro, BairroDTO>().ReverseMap();
                cfg.CreateMap<BairroViewModel, BairroDTO>().ReverseMap();
                cfg.CreateMap<Responsavel, ResponsavelDTO>().ReverseMap();
                cfg.CreateMap<ResponsavelViewModel, ResponsavelDTO>().ReverseMap();
                cfg.CreateMap<Endereco, EnderecoDTO>().ReverseMap();
                cfg.CreateMap<EnderecoViewModel, EnderecoDTO>().ReverseMap();

            });

            services.AddSingleton(autoMapperConfig.CreateMapper());

            #endregion
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

        }
    }

    public interface IStartup
    {
        IConfiguration Configuration { get; }
        void ConfigureServices(IServiceCollection services);
        void Configure(WebApplication app, IWebHostEnvironment env);
    }

    public static class StartupExtensions
    {
        public static WebApplicationBuilder UseStartup<TStartup>(this WebApplicationBuilder builder) where TStartup : IStartup
        {
            var startup = Activator.CreateInstance(typeof(TStartup), builder.Configuration) as IStartup;
            if (startup == null) throw new ArgumentException("Classe Startup.cs inválida!");

            startup.ConfigureServices(builder.Services);

            var app = builder.Build();
            startup.Configure(app, app.Environment);

            app.Run();

            return builder;
        }
    }
}
