using System;
using System.IO;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class AlunoFotoService : IAlunoFotoService
    {
        private readonly IAlunoRepository _repository;
        private readonly IFotoStorage _storage;

        public AlunoFotoService(IAlunoRepository repository, IFotoStorage storage)
        {
            _repository = repository;
            _storage = storage;
        }

        public async Task SalvarFoto(long alunoId, string nomeArquivo, string contentType, Stream conteudo)
        {
            var aluno = await _repository.GetByIdAsync(alunoId);
            if (aluno == null) throw new DomainException("Não existe um aluno com o ID informado!");
            if (conteudo == null || conteudo.Length == 0)
                throw new DomainException("Nenhuma imagem enviada.");

            var novoArquivoId = await _storage.UploadAsync(nomeArquivo, contentType, conteudo);
            var antigo = await _repository.DefinirFotoAsync(alunoId, novoArquivoId);
            if (!string.IsNullOrEmpty(antigo))
                await _storage.ExcluirAsync(antigo);
        }

        public async Task RemoverFoto(long alunoId)
        {
            var antigo = await _repository.DefinirFotoAsync(alunoId, null);
            if (!string.IsNullOrEmpty(antigo))
                await _storage.ExcluirAsync(antigo);
        }

        public async Task<string> ObterFotoDataUri(long alunoId)
        {
            var aluno = await _repository.GetByIdAsync(alunoId);
            if (aluno == null || string.IsNullOrEmpty(aluno.FotoArquivoId)) return null;

            var download = await _storage.BaixarAsync(aluno.FotoArquivoId);
            if (download == null) return null;

            using var ms = new MemoryStream();
            await download.Conteudo.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());
            return $"data:{download.ContentType};base64,{base64}";
        }

        public async Task<long?> ObterPoloId(long alunoId)
        {
            var aluno = await _repository.GetByIdAsync(alunoId);
            return aluno?.PoloId;
        }

        public async Task<ConfiguracaoFotoAlunoDTO> ObterConfig()
        {
            var cfg = await _repository.ObterConfigFotoAsync() ?? new ConfiguracaoFotoAluno();
            return new ConfiguracaoFotoAlunoDTO
            {
                MostrarNoCadastro = cfg.MostrarNoCadastro,
                MostrarNaChamada = cfg.MostrarNaChamada,
                MostrarNoResponsavel = cfg.MostrarNoResponsavel,
                MostrarNaCarteirinha = cfg.MostrarNaCarteirinha,
            };
        }

        public Task SalvarConfig(ConfiguracaoFotoAlunoDTO dto)
        {
            return _repository.SalvarConfigFotoAsync(new ConfiguracaoFotoAluno
            {
                MostrarNoCadastro = dto.MostrarNoCadastro,
                MostrarNaChamada = dto.MostrarNaChamada,
                MostrarNoResponsavel = dto.MostrarNoResponsavel,
                MostrarNaCarteirinha = dto.MostrarNaCarteirinha,
            });
        }
    }
}
