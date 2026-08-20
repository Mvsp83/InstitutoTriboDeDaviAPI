using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class DocumentoOficialService : IDocumentoOficialService
    {
        private readonly IMapper _mapper;
        private readonly IDocumentoOficialRepository _repository;

        public DocumentoOficialService(IMapper mapper, IDocumentoOficialRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<List<DocumentoOficialDTO>> ObterPorAno(int ano)
        {
            var docs = await _repository.ObterPorAnoAsync(ano);
            return _mapper.Map<List<DocumentoOficialDTO>>(docs);
        }

        public async Task<List<int>> ObterAnos() => await _repository.ObterAnosAsync();

        public async Task<DocumentoOficialDTO> Get(long id)
        {
            var doc = await _repository.GetByIdAsync(id);
            return _mapper.Map<DocumentoOficialDTO>(doc);
        }

        public async Task<DocumentoOficialDTO> Create(DocumentoOficialDTO dto)
        {
            var doc = _mapper.Map<DocumentoOficial>(dto);
            // Rascunho sempre nasce sem número.
            doc.Status = 0;
            doc.Numero = 0;
            doc.NumeroFormatado = string.Empty;
            doc.DataAprovacao = null;
            doc.Ano = doc.DataDocumento.Year;
            doc.Validate();
            var criado = await _repository.CreateAsync(doc);
            return _mapper.Map<DocumentoOficialDTO>(criado);
        }

        public async Task<DocumentoOficialDTO> Update(DocumentoOficialDTO dto)
        {
            var existente = await _repository.GetByIdAsync(dto.Id);
            if (existente == null)
                throw new DomainException("Documento não encontrado.");
            if (existente.Status == 1)
                throw new DomainException(
                    "Documento aprovado não pode ser alterado (numeração oficial).");

            var doc = _mapper.Map<DocumentoOficial>(dto);
            // Preserva o estado de rascunho; edição não mexe em número/status.
            doc.Status = 0;
            doc.Numero = 0;
            doc.NumeroFormatado = string.Empty;
            doc.DataAprovacao = null;
            doc.Ano = doc.DataDocumento.Year;
            doc.Validate();
            var atualizado = await _repository.UpdateAsync(doc);
            return _mapper.Map<DocumentoOficialDTO>(atualizado);
        }

        public async Task Delete(long id)
        {
            var existente = await _repository.GetByIdAsync(id);
            if (existente == null) return;
            if (existente.Status == 1)
                throw new DomainException(
                    "Documento aprovado não pode ser excluído (numeração oficial).");
            await _repository.DeleteAsync(id);
        }

        public async Task<DocumentoOficialDTO> Aprovar(long id)
        {
            var aprovado = await _repository.AprovarAsync(id);
            return _mapper.Map<DocumentoOficialDTO>(aprovado);
        }
    }
}
