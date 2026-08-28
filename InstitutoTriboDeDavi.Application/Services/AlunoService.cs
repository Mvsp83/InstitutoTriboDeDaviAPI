using AutoMapper;
using InstitutoTriboDeDavi.Application.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services.Interfaces;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly IMapper _mapper;
        private readonly IAlunoRepository _alunoRepository;

        public AlunoService(IMapper mapper, IAlunoRepository alunoRepository)
        {
            _mapper = mapper;
            _alunoRepository = alunoRepository;
        }

        public async Task<AlunoDTO> Create(AlunoDTO alunoDTO)
        {
            var alunoExists = await _alunoRepository.GetByNome(alunoDTO.Nome);

            if (alunoExists != null)
            {
                throw new DomainException("Já existe um registro com o mesmo Nome informado!");
            }

            var aluno = _mapper.Map<Aluno>(alunoDTO);

            aluno.Validate();

            var alunoCreated = await _alunoRepository.CreateAsync(aluno);

            return _mapper.Map<AlunoDTO>(alunoCreated);
        }
        public async Task Delete(long id)
        {
            await _alunoRepository.DeleteAsync(id);
        }

        public async Task<AlunoDTO> Get(long id)
        {
            var aluno = await _alunoRepository.GetByIdAsync(id);

            return _mapper.Map<AlunoDTO>(aluno);
        }

        public async Task<List<AlunoDTO>> GetAll()
        {
            var allAlunos = await _alunoRepository.GetAllAsync();

            return _mapper.Map<List<AlunoDTO>>(allAlunos);
        }

        public async Task<AlunoDTO> GetByNome(string nome)
        {
            var aluno = await _alunoRepository.GetByNome(nome);

            return _mapper.Map<AlunoDTO>(aluno);
        }

        public async Task<List<AlunoDTO>> SearchByNome(string nome)
        {
            var allAlunos = await _alunoRepository.SearchByNome(nome);

            return _mapper.Map<List<AlunoDTO>>(allAlunos);
        }

        public async Task<AlunoDTO> Update(AlunoDTO alunoDTO)
        {
            var alunoExists = await _alunoRepository.GetByIdAsync(alunoDTO.Id);

            if (alunoExists == null)
            {
                throw new DomainException("Não existe um registro com o ID informado!");
            }

            var aluno = _mapper.Map<Aluno>(alunoDTO);

            // O UpdateAsync grava a entidade inteira; como o formulário de edição
            // não traz estes campos, preserva-os para não os zerar.
            aluno.CodigoResponsavel = alunoExists.CodigoResponsavel;
            aluno.AnonimizadoEm = alunoExists.AnonimizadoEm;
            aluno.AutorizaImagemEm = alunoExists.AutorizaImagemEm;
            // Autorização de imagem: usa o valor enviado, ou preserva o atual.
            aluno.AutorizaImagem = alunoDTO.AutorizaImagem ?? alunoExists.AutorizaImagem;
            // "É adulto" vem da aprovação da inscrição, não da ficha — preserva.
            aluno.EhAdulto = alunoExists.EhAdulto;
            // A foto é gerida por endpoints próprios; a ficha não a traz — preserva.
            aluno.FotoArquivoId = alunoExists.FotoArquivoId;

            aluno.Validate();

            var alunoUpdated = await _alunoRepository.UpdateAsync(aluno);

            return _mapper.Map<AlunoDTO>(alunoUpdated);
        }

        public async Task<List<AlunoDTO>> ObterAlunosPorTurmaAsync(UsuarioDTO usuarioDTO, List<int> turmas)
        {
            if (usuarioDTO.Role == UserRole.Administrador)
            {
                var listaTodos = await _alunoRepository.ObterTodosAsync();

                return _mapper.Map<List<AlunoDTO>>(listaTodos);
            }

            // Supervisor e Professor enxergam apenas o próprio polo
            if ((usuarioDTO.Role == UserRole.Professor || usuarioDTO.Role == UserRole.Supervisor) && usuarioDTO.PoloId.HasValue)
            {
                var listaPorPolo = await _alunoRepository.ObterPorPoloTurmaAsync(usuarioDTO.PoloId.Value, turmas);

                return _mapper.Map<List<AlunoDTO>>(listaPorPolo);
            }

            throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
        }

        public async Task<List<AlunoPendenteDTO>> ObterAlunosPendentesAsync(UsuarioDTO usuarioDTO)
        {
            List<Aluno> pendentes;

            if (usuarioDTO.Role == UserRole.Administrador)
            {
                pendentes = await _alunoRepository.ObterTodosPendentesAsync();
            }
            else if ((usuarioDTO.Role == UserRole.Professor || usuarioDTO.Role == UserRole.Supervisor) && usuarioDTO.PoloId.HasValue)
            {
                pendentes = await _alunoRepository.ObterPendentesPorPoloAsync(usuarioDTO.PoloId.Value);
            }
            else
            {
                throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
            }

            return _mapper.Map<List<AlunoPendenteDTO>>(pendentes);
        }

        public async Task<AlunoDTO> AtribuirTurmaAsync(AtribuirTurmaDTO dto)
        {
            var aluno = await _alunoRepository.GetByIdAsync(dto.AlunoId);

            if (aluno == null)
                throw new DomainException("Nenhum aluno encontrado com o ID informado.");

            if (aluno.Turma != 0)
                throw new DomainException($"O aluno '{aluno.Nome}' já está na Turma {aluno.Turma}. Use o endpoint de Update para alterar.");

            aluno.Turma = dto.Turma;

            var atualizado = await _alunoRepository.UpdateAsync(aluno);

            return _mapper.Map<AlunoDTO>(atualizado);
        }

        // ── LGPD ──────────────────────────────────────────────────────────

        public async Task<DadosPessoaisAlunoDTO> ExportarDadosAsync(long id, string geradoPor)
        {
            var dados = await _alunoRepository.ColetarDadosPessoaisAsync(id);

            if (dados == null)
                return null;

            var a = dados.Aluno;

            return new DadosPessoaisAlunoDTO
            {
                GeradoEm = DateTime.Now,
                GeradoPor = geradoPor ?? string.Empty,
                Cadastro = new DadosCadastraisDTO
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    RG = a.RG,
                    CPF = a.CPF,
                    DataNascimento = a.DataNascimento,
                    Peso = a.Peso,
                    Altura = a.Altura,
                    Faixa = a.Faixa.ToString(),
                    Endereco = a.Endereco,
                    Numero = a.Numero,
                    Complemento = a.Complemento,
                    Bairro = a.Bairro,
                    Cidade = a.Cidade,
                    Celular = a.Celular,
                    Telefone2 = a.Telefone2,
                    Responsavel = a.Responsavel,
                    Parentesco = a.Parentesco?.ToString(),
                    RGResponsavel = a.RGResponsavel,
                    CPFResponsavel = a.CPFResponsavel,
                    Escola = a.Escola,
                    Serie = a.Serie,
                    Periodo = a.Periodo,
                    PoloId = a.PoloId,
                    Turma = a.Turma,
                    AnonimizadoEm = a.AnonimizadoEm,
                },
                Matriculas = dados.Matriculas.Select(m => new MatriculaResumoDTO
                {
                    Ano = m.Ano,
                    PoloId = m.PoloId,
                    Turma = m.Turma,
                    DataMatricula = m.DataMatricula,
                    Ativa = m.Ativa,
                    DataEncerramento = m.DataEncerramento,
                    MotivoEncerramento = m.MotivoEncerramento,
                }).ToList(),
                Graduacoes = dados.Graduacoes.Select(g => new GraduacaoResumoDTO
                {
                    Data = g.Data,
                    FaixaAnterior = g.FaixaAnterior,
                    FaixaNova = g.FaixaNova,
                    PoloId = g.PoloId,
                    Observacao = g.Observacao,
                    RegistradoPor = g.RegistradoPor,
                }).ToList(),
                Presencas = dados.Presencas.Select(p => new PresencaResumoDTO
                {
                    Data = p.Data,
                    EstaPresente = p.EstaPresente,
                    PoloId = p.PoloId,
                    Observacoes = p.Observacoes ?? string.Empty,
                }).ToList(),
                Inscricoes = dados.Inscricoes.Select(i => new InscricaoResumoDTO
                {
                    Ano = i.Ano,
                    Status = ((StatusInscricao)i.Status).ToString(),
                    DataEnvio = i.DataEnvio,
                    AceitouTermo = i.AceitouTermo,
                    AceitouImagem = i.AceitouImagem,
                    AceitouComodato = i.AceitouComodato,
                    AceitouLgpd = i.AceitouLgpd,
                    VersaoTermos = i.VersaoTermos,
                    NomeAssinatura = i.NomeAssinatura,
                }).ToList(),
            };
        }

        public async Task<AlunoDTO> AnonimizarAsync(long id)
        {
            var aluno = await _alunoRepository.AnonimizarAsync(id);

            return aluno == null ? null : _mapper.Map<AlunoDTO>(aluno);
        }

        public async Task<List<CandidatoRetencaoDTO>> ObterCandidatosRetencaoAsync(int mesesInativo)
        {
            var candidatos = await _alunoRepository.ObterCandidatosRetencaoAsync(mesesInativo);

            return candidatos.Select(c => new CandidatoRetencaoDTO
            {
                Id = c.Aluno.Id,
                Nome = c.Aluno.Nome,
                PoloId = c.Aluno.PoloId,
                UltimaPresenca = c.UltimaPresenca,
                UltimoAnoMatricula = c.UltimoAnoMatricula,
                MesesInativo = c.MesesInativo,
            }).ToList();
        }

        // ── Portal do responsável ───────────────────────────────────────────

        public async Task<string> GerarCodigoResponsavelAsync(long id)
        {
            var aluno = await _alunoRepository.GetByIdAsync(id);
            if (aluno == null)
                return null;

            var codigo = CodigoAcesso.Gerar();
            aluno.CodigoResponsavel = codigo;
            await _alunoRepository.UpdateAsync(aluno);

            return codigo;
        }

        public async Task<string?> ObterCodigoResponsavelAsync(long id)
        {
            var aluno = await _alunoRepository.GetByIdAsync(id);
            return aluno?.CodigoResponsavel;
        }

        public async Task<List<CodigoResponsavelItemDTO>> PrepararCodigosResponsavelAsync(UsuarioDTO usuario)
        {
            List<Aluno> alunos;
            if (usuario.Role == UserRole.Administrador)
            {
                alunos = (await _alunoRepository.ObterTodosAsync())
                    .Where(a => a.AnonimizadoEm == null)
                    .ToList();
            }
            else if ((usuario.Role == UserRole.Professor || usuario.Role == UserRole.Supervisor) && usuario.PoloId.HasValue)
            {
                alunos = await _alunoRepository.ObterPorPoloAsync(usuario.PoloId.Value);
            }
            else
            {
                throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
            }

            // Garante um código para cada aluno que ainda não tem.
            foreach (var aluno in alunos.Where(a => string.IsNullOrEmpty(a.CodigoResponsavel)))
            {
                aluno.CodigoResponsavel = CodigoAcesso.Gerar();
                await _alunoRepository.UpdateAsync(aluno);
            }

            return alunos
                .OrderBy(a => a.Nome)
                .Select(a => new CodigoResponsavelItemDTO
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    Responsavel = a.Responsavel ?? string.Empty,
                    PoloId = a.PoloId,
                    Codigo = a.CodigoResponsavel ?? string.Empty,
                }).ToList();
        }
    }
}
