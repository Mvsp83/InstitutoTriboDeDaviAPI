using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class OcorrenciaAlunoService : IOcorrenciaAlunoService
    {
        private const int TipoAdvertencia = 0;
        private const int TipoRecado = 1;

        private readonly IOcorrenciaAlunoRepository _repository;
        private readonly IAlunoRepository _alunoRepository;

        public OcorrenciaAlunoService(IOcorrenciaAlunoRepository repository, IAlunoRepository alunoRepository)
        {
            _repository = repository;
            _alunoRepository = alunoRepository;
        }

        public async Task<List<OcorrenciaAlunoDTO>> ListarPorAluno(long alunoId)
        {
            var itens = await _repository.ListarPorAlunoAsync(alunoId);
            return itens.Select(ToDTO).ToList();
        }

        public async Task<OcorrenciaAlunoDTO> Criar(OcorrenciaAlunoDTO dto, string registradoPor)
        {
            var aluno = await _alunoRepository.GetByIdAsync(dto.AlunoId)
                ?? throw new DomainException("Aluno não encontrado.");

            var texto = (dto.Texto ?? string.Empty).Trim();

            // Advertência precisa do motivo; recado pode ter só o status.
            if (dto.Tipo == TipoAdvertencia && texto.Length == 0)
                throw new DomainException("Escreva o motivo da advertência.");
            if (dto.Tipo != TipoAdvertencia && dto.Tipo != TipoRecado)
                throw new DomainException("Tipo de ocorrência inválido.");

            var criado = await _repository.CriarAsync(new OcorrenciaAluno
            {
                AlunoId = aluno.Id,
                PoloId = aluno.PoloId,
                Tipo = dto.Tipo,
                Status = dto.Tipo == TipoRecado ? dto.Status : 0,
                Texto = texto,
                Data = DateTime.Now,
                RegistradoPor = registradoPor ?? string.Empty,
            });

            return ToDTO(criado);
        }

        public async Task Excluir(long id)
        {
            await _repository.ExcluirAsync(id);
        }

        private static OcorrenciaAlunoDTO ToDTO(OcorrenciaAluno o) => new()
        {
            Id = o.Id,
            AlunoId = o.AlunoId,
            Tipo = o.Tipo,
            Status = o.Status,
            Texto = o.Texto,
            Data = o.Data,
            RegistradoPor = o.RegistradoPor,
        };
    }
}
