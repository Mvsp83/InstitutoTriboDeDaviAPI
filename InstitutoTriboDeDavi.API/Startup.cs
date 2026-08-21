using AutoMapper;
using InstitutoTriboDeDavi.API.BackgroundServices;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.Token;
using InstitutoTriboDeDavi.API.Token.Interfaces;
using InstitutoTriboDeDavi.API.ViewModels.Create;
using InstitutoTriboDeDavi.API.ViewModels.Usuario;
using InstitutoTriboDeDavi.Infrastructure.Repositories;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Entities.Consultas;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.DTO.Queries;
using InstitutoTriboDeDavi.Infrastructure.Import;
using InstitutoTriboDeDavi.Application.Import;
using InstitutoTriboDeDavi.Infrastructure.Configuration;
using InstitutoTriboDeDavi.Infrastructure.Context;
using InstitutoTriboDeDavi.Infrastructure.GoogleDrive;
using InstitutoTriboDeDavi.Infrastructure.GoogleSheets;
using InstitutoTriboDeDavi.Infrastructure.Email;
using InstitutoTriboDeDavi.Application.Services;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

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
            services.AddControllers(options =>
            {
                // Não trata string não-anulável como [Required] implícito. Os DTOs
                // têm campos preenchidos pelo servidor (ex.: CriadoPor,
                // NumeroFormatado) que o cliente não envia — sem isso, a validação
                // automática do model rejeitaria a requisição com 400.
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            });
            services.AddEndpointsApiExplorer();            

            #region Jwt

            var secretKey = Configuration["Jwt:Key"];
            var issuer = Configuration["Jwt:Issuer"];
            var audience = Configuration["Jwt:Audience"];

            // Fail-fast: sem chave (ou com chave fraca) a API não deve subir —
            // melhor um erro claro no deploy do que um 500 obscuro no primeiro login
            if (string.IsNullOrWhiteSpace(secretKey) || Encoding.ASCII.GetByteCount(secretKey) < 32)
                throw new InvalidOperationException(
                    "Jwt:Key ausente ou com menos de 32 bytes. " +
                    "Dev: dotnet user-secrets set \"Jwt:Key\" \"<chave>\". Produção: variável de ambiente Jwt__Key.");

            if (!int.TryParse(Configuration["Jwt:HoursToExpire"], out var hoursToExpire) || hoursToExpire <= 0)
                throw new InvalidOperationException("Jwt:HoursToExpire ausente ou inválido (esperado um inteiro positivo).");

            var isDevelopment = string.Equals(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                "Development", StringComparison.OrdinalIgnoreCase);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                // Em produção, metadata de autenticação só trafega por HTTPS
                x.RequireHttpsMetadata = !isDevelopment;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    // Issuer/Audience são validados quando configurados
                    // (produção define Jwt:Issuer e Jwt:Audience; dev não)
                    ValidateIssuer = !string.IsNullOrEmpty(issuer),
                    ValidIssuer = issuer,
                    ValidateAudience = !string.IsNullOrEmpty(audience),
                    ValidAudience = audience
                };
            });

            #endregion

            #region Rate Limiting

            // Proteção contra força bruta no login: 5 tentativas por minuto por IP.
            // O IP real chega via X-Forwarded-For (tratado pelo UseForwardedHeaders).
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy(AuthPolicies.LoginRateLimit, context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "desconhecido",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            PermitLimit = 5,
                            QueueLimit = 0
                        }));
            });

            #endregion

            #region Autorização

            services.AddAuthorization(options =>
            {
                // Fallback: todo endpoint exige usuário autenticado por padrão;
                // exceções (login, setup) usam [AllowAnonymous] explícito
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy(AuthPolicies.ProfessorOuSuperior, policy =>
                    policy.RequireRole(
                        nameof(UserRole.Administrador),
                        nameof(UserRole.Supervisor),
                        nameof(UserRole.Professor)));
            });

            #endregion

            #region Injeção de Dependência

            services.AddDbContext<TriboDeDaviContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:TRIBODEDAVIAPI"]));

            // Health check com verificação do banco — usado pelo monitoramento do provedor
            services.AddHealthChecks().AddDbContextCheck<TriboDeDaviContext>();

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
            services.AddScoped<IPlanoDeAulaRepository, PlanoDeAulaRepository>();
            services.AddScoped<IPlanoDeAulaService, PlanoDeAulaService>();
            services.AddScoped<IModeloDeAulaRepository, ModeloDeAulaRepository>();
            services.AddScoped<IModeloDeAulaService, ModeloDeAulaService>();
            services.AddScoped<IAtividadeRepository, AtividadeRepository>();
            services.AddScoped<IAtividadeService, AtividadeService>();
            services.AddScoped<IRelatorioSalvoRepository, RelatorioSalvoRepository>();
            services.AddScoped<IRelatorioSalvoService, RelatorioSalvoService>();
            services.AddScoped<IFrequenciaRepository, FrequenciaRepository>();
            services.AddScoped<IFrequenciaService, FrequenciaService>();
            services.AddScoped<IAniversarianteRepository, AniversarianteRepository>();
            services.AddScoped<IAniversarianteService, AniversarianteService>();
            services.AddScoped<IConfiguracaoDocumentoRepository, ConfiguracaoDocumentoRepository>();
            services.AddScoped<IConfiguracaoDocumentoService, ConfiguracaoDocumentoService>();
            services.AddScoped<IEventoCalendarioRepository, EventoCalendarioRepository>();
            services.AddScoped<IEventoCalendarioService, EventoCalendarioService>();
            services.AddScoped<IDocumentoOficialRepository, DocumentoOficialRepository>();
            services.AddScoped<IDocumentoOficialService, DocumentoOficialService>();
            services.AddScoped<IBemPatrimonialRepository, BemPatrimonialRepository>();
            services.AddScoped<IBemPatrimonialService, BemPatrimonialService>();
            services.AddScoped<IAvisoRepository, AvisoRepository>();
            services.AddScoped<IAvisoService, AvisoService>();

            services.AddScoped<ITokenGenerator, TokenGenerator>();

            services.AddScoped<IPasswordHasher<UsuarioDTO>, PasswordHasher<UsuarioDTO>>();
            // Configuração do Google Sheets
            services.Configure<GoogleSheetsConfig>(Configuration.GetSection("GoogleSheets"));

            // Configuração do Google Drive (documentos contábeis)
            services.Configure<GoogleDriveConfig>(Configuration.GetSection("GoogleDrive"));

            // Configuração de email (avisos do calendário)
            services.Configure<SmtpConfig>(Configuration.GetSection("Smtp"));

            // Serviços
            services.AddScoped<IGoogleSheetsService, GoogleSheetsService>();
            services.AddScoped<IDocumentoDriveService, GoogleDriveDocumentoService>();
            services.AddScoped<IEmailService, EmailService>();

            // Background service de sincronização automática
            services.AddHostedService<SincronizacaoHostedService>();

            // Background service dos avisos do calendário por email
            services.AddHostedService<NotificacaoCalendarioHostedService>();

            services.AddScoped<IFactoryPlanilhaDB, FactoryPlanilhaDB>();
            services.AddScoped<ISincronizacaoHistoricoRepository, SincronizacaoHistoricoRepository>();

            #endregion

            #region AutoMapper

            services.AddSingleton<IMapper>(sp =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Usuario, UsuarioDTO>().ReverseMap();
                    cfg.CreateMap<UsuarioViewModel, UsuarioDTO>().ReverseMap();
                    cfg.CreateMap<Aluno, AlunoDTO>().ReverseMap();
                    cfg.CreateMap<Polo, PoloDTO>().ReverseMap();
                    cfg.CreateMap<ConfiguracaoDocumento, ConfiguracaoDocumentoDTO>().ReverseMap();
                    cfg.CreateMap<EventoCalendario, EventoCalendarioDTO>().ReverseMap();
                    cfg.CreateMap<DocumentoOficial, DocumentoOficialDTO>().ReverseMap();
                    cfg.CreateMap<BemPatrimonial, BemPatrimonialDTO>().ReverseMap();
                    cfg.CreateMap<Aviso, AvisoDTO>().ReverseMap();

                    cfg.CreateMap<Presenca, PresencaDTO>().ReverseMap();
                    cfg.CreateMap<Aula, AulaDTO>().ReverseMap();

                    cfg.CreateMap<PlanoDeAula, PlanoDeAulaDTO>().ReverseMap();
                    cfg.CreateMap<BlocoDoPlano, BlocoDoPlanoDTO>().ReverseMap();
                    cfg.CreateMap<ModeloDeAula, ModeloDeAulaDTO>().ReverseMap();
                    cfg.CreateMap<BlocoDoModelo, BlocoDoModeloDTO>().ReverseMap();
                    cfg.CreateMap<Atividade, AtividadeDTO>().ReverseMap();
                    cfg.CreateMap<AtividadeDoBloco, AtividadeDoBlocoDTO>().ReverseMap();
                    cfg.CreateMap<RelatorioSalvo, RelatorioSalvoDTO>().ReverseMap();
                    cfg.CreateMap<HistoricoAtividade, HistoricoAtividadeDTO>().ReverseMap();

                    cfg.CreateMap<Frequencia, FrequenciaDTO>().ReverseMap();
                    cfg.CreateMap<Aniversariante, AniversarianteDTO>().ReverseMap();

                    cfg.CreateMap<LoginViewModel, UsuarioDTO>().ReverseMap();
                    cfg.CreateMap<Aluno, AlunoPendenteDTO>().ReverseMap();

                }, sp.GetRequiredService<ILoggerFactory>());

                return config.CreateMapper();
            });

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
            else
            {
                // Atrás de proxy gerenciado (App Service, etc.) o TLS termina antes
                // do Kestrel; os headers X-Forwarded-* preservam esquema e IP reais
                var forwardedOptions = new ForwardedHeadersOptions
                {
                    ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                };
                forwardedOptions.KnownNetworks.Clear();
                forwardedOptions.KnownProxies.Clear();
                app.UseForwardedHeaders(forwardedOptions);

                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRateLimiter();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            // Liveness/readiness anônimo para o monitoramento
            app.MapHealthChecks("/health").AllowAnonymous();
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

            // CORS por ambiente: as origens permitidas vêm da configuração
            // (Cors:AllowedOrigins). O app mobile não usa CORS — isso existe
            // para um eventual front web; sem configuração, nada é liberado.
            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Default", policy =>
                {
                    if (allowedOrigins.Length > 0)
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                });
            });

            var app = builder.Build();
            app.UseCors("Default");
            startup.Configure(app, app.Environment);

            app.Run();

            return builder;
        }
    }
}
