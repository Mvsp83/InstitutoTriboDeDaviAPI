using System.Text.RegularExpressions;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class MetricaService : IMetricaService
    {
        private readonly IMetricaRepository _repo;

        public MetricaService(IMetricaRepository repo)
        {
            _repo = repo;
        }

        // Eventos aceitos. Qualquer outro é ignorado.
        private static readonly HashSet<string> EventosSimples = new()
        {
            "doar_click", "inscricao_ok", "responsavel_acesso",
        };

        // Dispositivos aceitos (lista fechada — evita chaves arbitrárias).
        private static readonly HashSet<string> Dispositivos = new()
        {
            "celular", "tablet", "desktop",
        };

        public async Task RegistrarAsync(MetricaEventoDTO evento)
        {
            var nome = (evento?.Evento ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(nome)) return;

            string chave;

            if (nome == "pageview")
            {
                chave = $"pageview:{NormalizarCaminho(evento.Dimensao)}";
            }
            else if (nome == "davizinho")
            {
                var texto = NormalizarTexto(evento.Dimensao);
                if (string.IsNullOrEmpty(texto)) return;
                chave = $"davizinho:{texto}";
            }
            else if (nome == "origem")
            {
                var slug = NormalizarSlug(evento.Dimensao);
                chave = $"origem:{(string.IsNullOrEmpty(slug) ? "direto" : slug)}";
            }
            else if (nome == "dispositivo")
            {
                var d = (evento.Dimensao ?? "").Trim().ToLowerInvariant();
                if (!Dispositivos.Contains(d)) return;
                chave = $"dispositivo:{d}";
            }
            else if (nome == "inscricao_etapa")
            {
                var slug = NormalizarSlug(evento.Dimensao);
                if (string.IsNullOrEmpty(slug)) return;
                chave = $"funil:{slug}";
            }
            else if (EventosSimples.Contains(nome))
            {
                chave = $"evento:{nome}";
            }
            else
            {
                return; // fora da lista branca
            }

            await _repo.IncrementarAsync(DateTime.Now, chave, 1);
        }

        public async Task<MetricaResumoDTO> ObterResumoAsync(int dias)
        {
            if (dias < 1) dias = 1;
            if (dias > 365) dias = 365;

            var hoje = DateTime.Now.Date;
            var desde = hoje.AddDays(-(dias - 1));
            var linhas = await _repo.ObterDesdeAsync(desde);

            long SomaPrefixo(string prefixo) =>
                linhas.Where(l => l.Chave.StartsWith(prefixo)).Sum(l => l.Valor);

            long SomaChave(string chave) =>
                linhas.Where(l => l.Chave == chave).Sum(l => l.Valor);

            List<ItemContagemDTO> TopPorPrefixo(string prefixo, int qtd) =>
                linhas.Where(l => l.Chave.StartsWith(prefixo))
                    .GroupBy(l => l.Chave.Substring(prefixo.Length))
                    .Select(g => new ItemContagemDTO { Rotulo = g.Key, Valor = g.Sum(x => x.Valor) })
                    .OrderByDescending(i => i.Valor)
                    .Take(qtd)
                    .ToList();

            // Série de visitas por dia, com zeros nos dias sem acesso.
            var visitasPorDiaMapa = linhas
                .Where(l => l.Chave.StartsWith("pageview:"))
                .GroupBy(l => l.Data.Date)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Valor));

            var serie = new List<SerieDiaDTO>();
            for (var d = desde; d <= hoje; d = d.AddDays(1))
                serie.Add(new SerieDiaDTO
                {
                    Data = d,
                    Valor = visitasPorDiaMapa.TryGetValue(d, out var v) ? v : 0,
                });

            var inscricoesOk = SomaChave("evento:inscricao_ok");

            // Funil: etapas (maior → menor, forma de funil) + a conclusão no fim.
            var funil = TopPorPrefixo("funil:", 20);
            funil.Add(new ItemContagemDTO { Rotulo = "concluída", Valor = inscricoesOk });

            return new MetricaResumoDTO
            {
                Dias = dias,
                Visitas = SomaPrefixo("pageview:"),
                DoarCliques = SomaChave("evento:doar_click"),
                InscricoesConcluidas = inscricoesOk,
                AcessosResponsavel = SomaChave("evento:responsavel_acesso"),
                VisitasPorDia = serie,
                TopPaginas = TopPorPrefixo("pageview:", 10),
                TopDavizinho = TopPorPrefixo("davizinho:", 10),
                Origem = TopPorPrefixo("origem:", 8),
                Dispositivos = TopPorPrefixo("dispositivo:", 5),
                FunilInscricao = funil,
            };
        }

        // Caminho da página: só o path (sem query/âncora), minúsculo, curto e
        // com caracteres seguros. Evita explosão de chaves e entradas estranhas.
        private static string NormalizarCaminho(string bruto)
        {
            var s = (bruto ?? "/").Trim();
            var corte = s.IndexOfAny(new[] { '?', '#' });
            if (corte >= 0) s = s.Substring(0, corte);
            if (string.IsNullOrEmpty(s)) s = "/";
            if (!s.StartsWith("/")) s = "/" + s;
            s = s.ToLowerInvariant();
            s = Regex.Replace(s, "[^a-z0-9/_-]", "");
            if (string.IsNullOrEmpty(s)) s = "/";
            if (s.Length > 100) s = s.Substring(0, 100);
            return s;
        }

        // Rótulo curto (origem, etapa do funil): minúsculo, espaços viram "_",
        // só caracteres seguros e tamanho bem limitado. Bom para chave/exibição.
        private static string NormalizarSlug(string bruto)
        {
            var s = (bruto ?? "").Trim().ToLowerInvariant();
            s = Regex.Replace(s, "\\s+", "_");
            // Mantém letras acentuadas do português (à-ÿ) para rótulos legíveis.
            s = Regex.Replace(s, "[^a-z0-9à-ÿ_-]", "");
            if (s.Length > 40) s = s.Substring(0, 40);
            return s;
        }

        // Texto livre (pergunta do Davizinho): minúsculo, espaços colapsados e
        // tamanho limitado, para agrupar variações parecidas e limitar chaves.
        private static string NormalizarTexto(string bruto)
        {
            var s = (bruto ?? "").Trim().ToLowerInvariant();
            s = Regex.Replace(s, "\\s+", " ");
            if (s.Length > 120) s = s.Substring(0, 120);
            return s;
        }
    }
}
