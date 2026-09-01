using InstitutoTriboDeDavi.API;

// Npgsql: mantém o comportamento clássico de data/hora (aceita DateTime.Now /
// Kind=Unspecified sem exigir UTC). Precisa ser definido antes de qualquer uso
// do Npgsql. Necessário na migração de SQL Server para PostgreSQL.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args)
    .UseStartup<Startup>();
