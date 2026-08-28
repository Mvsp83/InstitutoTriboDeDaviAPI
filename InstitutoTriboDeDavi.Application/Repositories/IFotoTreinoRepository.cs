using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IFotoTreinoRepository
    {
        Task<FotoTreino> ObterAsync(long id);
        // A trava 1/turma/aula: acha a foto existente daquele recorte.
        Task<FotoTreino> ObterPorAulaAsync(long poloId, int turma, DateTime dataAula);
        Task<FotoTreino> AdicionarAsync(FotoTreino foto);
        Task<FotoTreino> AtualizarAsync(FotoTreino foto);
        Task ExcluirAsync(long id);
        Task DefinirPublicacaoAsync(long id, bool publicada);

        // Listagens já com o nome do polo (join), para os DTOs.
        Task<List<(FotoTreino foto, string poloNome)>> ListarTodasAsync();
        Task<List<(FotoTreino foto, string poloNome)>> ListarPublicasAsync();

        // Config por polo do fluxo de publicação.
        Task<PoloFotoConfig> ObterConfigAsync(long poloId);
        Task DefinirConfigAsync(long poloId, bool requerAutorizacao);
        // Todos os polos com sua config (default requer=true quando ausente).
        Task<List<(long poloId, string poloNome, bool requerAutorizacao)>> ListarConfigPolosAsync();
    }
}
