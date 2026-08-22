using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace InstitutoTriboDeDavi.Infrastructure.Auditoria
{
    // Grava o log de auditoria automaticamente a cada SaveChanges. Ficar num
    // interceptor — e não espalhado pelos serviços — garante que nenhuma
    // alteração das entidades sensíveis escape por esquecimento.
    public class AuditoriaInterceptor : SaveChangesInterceptor
    {
        // Só o que exige prestação de contas é auditado. Registrar tudo (aulas,
        // planos, avisos) só encheria a tabela sem valor para a ONG.
        private static readonly HashSet<string> Auditadas = new()
        {
            nameof(ContaFinanceira), nameof(MovimentacaoFinanceira),
            nameof(Doador), nameof(Doacao),
            nameof(Aluno), nameof(Usuario),
            nameof(Presenca), nameof(Graduacao),
        };

        private readonly IHttpContextAccessor _http;

        // Auditorias pendentes entre o "vou salvar" e o "salvei" — os ids de
        // registros novos só existem depois do salvamento.
        private readonly List<PendenteAuditoria> _pendentes = new();

        public AuditoriaInterceptor(IHttpContextAccessor http)
        {
            _http = http;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null) Coletar(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context != null) Coletar(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            await GravarAsync(eventData.Context, cancellationToken);
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            GravarAsync(eventData.Context, CancellationToken.None).GetAwaiter().GetResult();
            return base.SavedChanges(eventData, result);
        }

        private void Coletar(DbContext context)
        {
            var login = _http.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "sistema";
            var ip = _http.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "";

            foreach (var entry in context.ChangeTracker.Entries())
            {
                var nome = entry.Metadata.ClrType.Name;
                if (!Auditadas.Contains(nome)) continue;

                string acao = entry.State switch
                {
                    EntityState.Added => "Criou",
                    EntityState.Modified => "Alterou",
                    EntityState.Deleted => "Excluiu",
                    _ => null,
                };
                if (acao == null) continue;

                _pendentes.Add(new PendenteAuditoria
                {
                    Entry = entry,
                    Acao = acao,
                    Entidade = nome,
                    Login = login,
                    Ip = ip,
                    Alteracoes = Diferencas(entry),
                });
            }
        }

        private async Task GravarAsync(DbContext context, CancellationToken ct)
        {
            if (context == null || _pendentes.Count == 0) return;

            var logs = _pendentes.Select(p => new LogAuditoria
            {
                Data = DateTime.UtcNow,
                UsuarioLogin = p.Login,
                Acao = p.Acao,
                Entidade = p.Entidade,
                // Agora o id do registro novo já existe.
                EntidadeId = LerId(p.Entry),
                Resumo = $"{p.Acao} {p.Entidade} #{LerId(p.Entry)}",
                Alteracoes = p.Alteracoes,
                Ip = p.Ip,
            }).ToList();

            _pendentes.Clear();

            // Salvamento próprio: como LogAuditoria não está no conjunto
            // auditado, este SaveChanges não dispara o interceptor de novo.
            context.Set<LogAuditoria>().AddRange(logs);
            await context.SaveChangesAsync(ct);
        }

        private static long LerId(EntityEntry entry)
        {
            var pk = entry.Metadata.FindPrimaryKey()?.Properties.FirstOrDefault();
            if (pk == null) return 0;
            var valor = entry.Property(pk.Name).CurrentValue;
            return valor is long l ? l : Convert.ToInt64(valor ?? 0);
        }

        // Só os campos que mudaram, para o log ser legível e enxuto. Senhas e
        // hashes nunca entram no registro.
        private static string Diferencas(EntityEntry entry)
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Deleted)
                return string.Empty;

            var mudancas = new Dictionary<string, object>();
            foreach (var prop in entry.Properties)
            {
                if (!prop.IsModified) continue;
                var nome = prop.Metadata.Name;
                if (nome.Contains("Senha", StringComparison.OrdinalIgnoreCase) ||
                    nome.Contains("Hash", StringComparison.OrdinalIgnoreCase) ||
                    nome.Contains("Password", StringComparison.OrdinalIgnoreCase))
                    continue;

                mudancas[nome] = new { de = prop.OriginalValue, para = prop.CurrentValue };
            }

            return mudancas.Count == 0 ? string.Empty : JsonSerializer.Serialize(mudancas);
        }

        private sealed class PendenteAuditoria
        {
            public EntityEntry Entry { get; init; }
            public string Acao { get; init; }
            public string Entidade { get; init; }
            public string Login { get; init; }
            public string Ip { get; init; }
            public string Alteracoes { get; init; }
        }
    }
}
