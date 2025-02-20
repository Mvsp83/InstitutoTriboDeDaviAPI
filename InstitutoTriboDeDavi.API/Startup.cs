using AutoMapper;
using InstitutoTriboDeDavi.API.Token;
using InstitutoTriboDeDavi.API.Token.Interfaces;
using InstitutoTriboDeDavi.API.ViewModels.Create;
using InstitutoTriboDeDavi.API.ViewModels.Usuario;
using InstitutoTriboDeDavi.System.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Business;
using InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Domain.Entities.Business;
using InstitutoTriboDeDavi.System.Domain.Entities.Consultas;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.DTO.Business;
using InstitutoTriboDeDavi.System.DTO.Queries;
using InstitutoTriboDeDavi.System.Factory;
using InstitutoTriboDeDavi.System.Infra.Context;
using InstitutoTriboDeDavi.System.Services;
using InstitutoTriboDeDavi.System.Services.Business;
using InstitutoTriboDeDavi.System.Services.Business.Interfaces;
using InstitutoTriboDeDavi.System.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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

            #region Jwt

            var secretKey = Configuration["Jwt:Key"];

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            #endregion



            #region Injeção de Dependência

            services.AddSingleton(d => Configuration);
            services.AddDbContext<TriboDeDaviContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:TRIBODEDAVIAPI"]), ServiceLifetime.Transient);

            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IAlunoRepository, AlunoRepository>();
            services.AddScoped<IAlunoService, AlunoService>();
            services.AddScoped<IPoloRepository, PoloRepository>();
            services.AddScoped<IPoloService, PoloService>();

            services.AddScoped<IPresencaRepository, PresencaRepository>();
            services.AddScoped<IPresencaService, PresencaService>();
            services.AddScoped<IAulaRepository, AulaRepository>();
            services.AddScoped<IAulaService, AulaService>();
            services.AddScoped<IFrequenciaRepository, FrequenciaRepository>();
            services.AddScoped<IFrequenciaService, FrequenciaService>();
            services.AddScoped<IAniversarianteRepository, AniversarianteRepository>();
            services.AddScoped<IAniversarianteService, AniversarianteService>();

            services.AddScoped<ITokenGenerator, TokenGenerator>();

            services.AddScoped<IPasswordHasher<UsuarioDTO>, PasswordHasher<UsuarioDTO>>();


            services.AddTransient<FactoryPlanilhaDB>(provider =>
            new FactoryPlanilhaDB(Configuration.GetConnectionString("ConnectionStrings:TRIBODEDAVIAPI")));

            #endregion

            #region AutoMapper

            var autoMapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Usuario, UsuarioDTO>().ReverseMap();
                cfg.CreateMap<UsuarioViewModel, UsuarioDTO>().ReverseMap();
                cfg.CreateMap<Aluno, AlunoDTO>().ReverseMap();
                cfg.CreateMap<Polo, PoloDTO>().ReverseMap();

                cfg.CreateMap<Presenca, PresencaDTO>().ReverseMap();
                cfg.CreateMap<Aula, AulaDTO>().ReverseMap();                          

                cfg.CreateMap<Frequencia, FrequenciaDTO>().ReverseMap();
                cfg.CreateMap<Aniversariante, AniversarianteDTO>().ReverseMap();

                cfg.CreateMap<LoginViewModel, UsuarioDTO>().ReverseMap();

            });

            services.AddSingleton(autoMapperConfig.CreateMapper());

            #endregion


            #region Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "API Instituto Tribo de Davi",
                    Version = "v1",
                    Description = "API Operacional - Controle de Chamadas",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Name",
                        Email = "email",
                    },
                });

                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Por favor utilize Bearer <TOKEN>",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });          

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

            app.UseAuthentication();

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
