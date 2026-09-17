using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class RecadoService : IRecadoService
    {
        // Validade padrão de um recado quando o autor não informa uma data.
        private const int DiasValidadePadrao = 45;

        private readonly IMapper _mapper;
        private readonly IRecadoRepository _repository;
        private readonly IDenunciaRecadoRepository _denuncias;

        public RecadoService(
            IMapper mapper,
            IRecadoRepository repository,
            IDenunciaRecadoRepository denuncias)
        {
            _mapper = mapper;
            _repository = repository;
            _denuncias = denuncias;
        }

        public async Task<List<RecadoDTO>> ListarVigentes()
            => _mapper.Map<List<RecadoDTO>>(await _repository.ObterVigentesAsync());

        public async Task<List<RecadoDTO>> ListarTodos()
            => _mapper.Map<List<RecadoDTO>>(await _repository.ObterTodosAsync());

        public async Task<RecadoDTO> Obter(long id)
        {
            var recado = await _repository.GetByIdAsync(id);
            if (recado == null)
                throw new DomainException("Recado não encontrado.");
            return _mapper.Map<RecadoDTO>(recado);
        }

        public async Task<RecadoDTO> Create(RecadoDTO dto, UsuarioDTO usuario)
        {
            var recado = _mapper.Map<Recado>(dto);
            var agora = DateTime.Now;

            recado.Id = 0;
            recado.DataCriacao = agora;
            recado.CriadoPor = usuario.Login;
            recado.PoloId = usuario.PoloId; // polo de origem do autor
            recado.Ativo = true;
            recado.ExpiraEm = ValidadeOuPadrao(dto.ExpiraEm, agora);

            recado.Validate();
            var criado = await _repository.CreateAsync(recado);
            return _mapper.Map<RecadoDTO>(criado);
        }

        public async Task<RecadoDTO> Update(RecadoDTO dto, UsuarioDTO usuario)
        {
            var existente = await _repository.GetByIdAsync(dto.Id);
            if (existente == null)
                throw new DomainException("Recado não encontrado.");

            GarantirDono(existente, usuario);

            // Só os campos editáveis; autoria, origem e data de criação preservadas.
            existente.Titulo = dto.Titulo;
            existente.Descricao = dto.Descricao;
            existente.Categoria = dto.Categoria;
            existente.Anunciante = dto.Anunciante ?? string.Empty;
            existente.Contato = dto.Contato;
            existente.FotoArquivoId = dto.FotoArquivoId ?? string.Empty;
            existente.Ativo = dto.Ativo;
            existente.ExpiraEm = ValidadeOuPadrao(dto.ExpiraEm, existente.DataCriacao);

            existente.Validate();
            var atualizado = await _repository.UpdateAsync(existente);
            return _mapper.Map<RecadoDTO>(atualizado);
        }

        public async Task Delete(long id, UsuarioDTO usuario)
        {
            var existente = await _repository.GetByIdAsync(id);
            if (existente == null)
                throw new DomainException("Recado não encontrado.");

            GarantirDono(existente, usuario);
            await _denuncias.RemoverPorRecadoAsync(id); // limpa a fila do recado
            await _repository.DeleteAsync(id);
        }

        public async Task Denunciar(long recadoId, string motivo, string quem)
        {
            var recado = await _repository.GetByIdAsync(recadoId);
            if (recado == null)
                throw new DomainException("Recado não encontrado.");

            var denuncia = new DenunciaRecado
            {
                RecadoId = recadoId,
                Motivo = (motivo ?? string.Empty).Trim(),
                DenunciadoPor = quem ?? string.Empty,
                DataCriacao = DateTime.Now,
                Resolvida = false,
            };
            denuncia.Validate();
            await _denuncias.CreateAsync(denuncia);
        }

        public async Task<List<DenunciaRecadoDTO>> ListarDenunciasPendentes()
        {
            var pendentes = await _denuncias.ListarPendentesAsync();
            var lista = new List<DenunciaRecadoDTO>(pendentes.Count);
            foreach (var d in pendentes)
            {
                var recado = await _repository.GetByIdAsync(d.RecadoId);
                lista.Add(new DenunciaRecadoDTO
                {
                    Id = d.Id,
                    RecadoId = d.RecadoId,
                    RecadoTitulo = recado?.Titulo ?? "(recado removido)",
                    Motivo = d.Motivo,
                    DenunciadoPor = d.DenunciadoPor,
                    DataCriacao = d.DataCriacao,
                });
            }
            return lista;
        }

        public async Task ResolverDenuncia(long denunciaId, string login)
        {
            var denuncia = await _denuncias.GetByIdAsync(denunciaId);
            if (denuncia == null)
                throw new DomainException("Denúncia não encontrada.");

            denuncia.Resolvida = true;
            denuncia.ResolvidoPor = login ?? string.Empty;
            denuncia.DataResolucao = DateTime.Now;
            await _denuncias.UpdateAsync(denuncia);
        }

        // Só o autor do recado ou um Administrador podem editar/remover.
        private static void GarantirDono(Recado recado, UsuarioDTO usuario)
        {
            if (usuario.Role == UserRole.Administrador)
                return;
            if (!string.Equals(recado.CriadoPor, usuario.Login, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException(
                    "Só o autor do recado ou um administrador podem alterá-lo.");
        }

        private static DateTime ValidadeOuPadrao(DateTime? informada, DateTime baseData)
            => (informada.HasValue && informada.Value > baseData)
                ? informada.Value
                : baseData.AddDays(DiasValidadePadrao);
    }
}
