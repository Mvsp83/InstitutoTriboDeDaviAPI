using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class SolicitacaoInternaService : ISolicitacaoInternaService
    {
        private readonly IMapper _mapper;
        private readonly ISolicitacaoInternaRepository _repository;

        public SolicitacaoInternaService(IMapper mapper, ISolicitacaoInternaRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<List<SolicitacaoInternaDTO>> Listar(UsuarioDTO usuario)
        {
            var lista = VeTudo(usuario)
                ? await _repository.ListarTodasAsync()
                : await _repository.ListarDoUsuarioAsync(usuario.Login);
            return _mapper.Map<List<SolicitacaoInternaDTO>>(lista);
        }

        public async Task<SolicitacaoInternaDTO> Obter(long id, UsuarioDTO usuario)
        {
            var solicitacao = await _repository.ObterComMensagensAsync(id);
            if (solicitacao == null || !PodeVer(solicitacao, usuario))
                return null;
            return _mapper.Map<SolicitacaoInternaDTO>(solicitacao);
        }

        public async Task<SolicitacaoInternaDTO> Criar(CriarSolicitacaoDTO dados, UsuarioDTO usuario)
        {
            var agora = DateTime.Now;
            var eProfessor = usuario.Role != UserRole.Administrador
                             && usuario.Role != UserRole.Supervisor;

            var solicitacao = new SolicitacaoInterna
            {
                Assunto = dados.Assunto,
                Categoria = dados.Categoria,
                Status = (int)StatusSolicitacao.Aberta,
                CriadoPorLogin = usuario.Login,
                CriadoPorRole = (int)usuario.Role,
                DataCriacao = agora,
                DataAtualizacao = agora,
                Ativo = true,
                // Professor abre para a administração (sem destinatário), com o
                // próprio polo. Admin/supervisor mira um professor e o polo dele.
                DestinatarioLogin = eProfessor ? null : dados.DestinatarioLogin,
                PoloId = eProfessor ? usuario.PoloId : dados.PoloId,
                PoloNome = eProfessor ? usuario.PoloNome : dados.PoloNome,
            };
            solicitacao.Validate();

            // A primeira mensagem é o corpo do pedido.
            solicitacao.Mensagens.Add(new MensagemSolicitacao
            {
                AutorLogin = usuario.Login,
                AutorRole = (int)usuario.Role,
                Texto = dados.Texto,
                DataEnvio = agora,
            });
            solicitacao.Mensagens.ForEach(m => m.Validate());

            var criada = await _repository.CreateAsync(solicitacao);
            return _mapper.Map<SolicitacaoInternaDTO>(criada);
        }

        public async Task<SolicitacaoInternaDTO> Responder(long id, string texto, UsuarioDTO usuario)
        {
            var solicitacao = await _repository.ObterComMensagensAsync(id);
            if (solicitacao == null || !PodeVer(solicitacao, usuario))
                return null;

            var agora = DateTime.Now;
            var mensagem = new MensagemSolicitacao
            {
                SolicitacaoInternaId = solicitacao.Id,
                AutorLogin = usuario.Login,
                AutorRole = (int)usuario.Role,
                Texto = texto,
                DataEnvio = agora,
            };
            mensagem.Validate();
            await _repository.AdicionarMensagemAsync(mensagem);

            // Toda resposta reaquece a solicitação; se estava só "Aberta", passa a
            // "Em andamento" (alguém já pegou pra tratar).
            solicitacao.DataAtualizacao = agora;
            if (solicitacao.Status == (int)StatusSolicitacao.Aberta)
                solicitacao.Status = (int)StatusSolicitacao.EmAndamento;
            solicitacao.Mensagens = null; // evita reinserir a conversa no update
            await _repository.UpdateAsync(solicitacao);

            return await Obter(id, usuario);
        }

        public async Task<SolicitacaoInternaDTO> AlterarStatus(long id, int status, UsuarioDTO usuario)
        {
            var solicitacao = await _repository.ObterComMensagensAsync(id);
            if (solicitacao == null)
                return null;

            // Status é gerido por quem trata (admin/supervisor) ou por quem abriu
            // (pode reabrir/encerrar o próprio pedido).
            var podeGerir = VeTudo(usuario) || solicitacao.CriadoPorLogin == usuario.Login;
            if (!podeGerir)
                return null;

            solicitacao.Status = status;
            solicitacao.DataAtualizacao = DateTime.Now;
            solicitacao.Mensagens = null;
            await _repository.UpdateAsync(solicitacao);

            return await Obter(id, usuario);
        }

        public async Task<int> ContarNaoResolvidas(UsuarioDTO usuario)
        {
            return VeTudo(usuario)
                ? await _repository.ContarNaoResolvidasTodasAsync()
                : await _repository.ContarNaoResolvidasDoUsuarioAsync(usuario.Login);
        }

        // Admin e supervisor enxergam a caixa toda; os demais, só o que os toca.
        private static bool VeTudo(UsuarioDTO usuario) =>
            usuario.Role == UserRole.Administrador || usuario.Role == UserRole.Supervisor;

        private static bool PodeVer(SolicitacaoInterna s, UsuarioDTO usuario) =>
            VeTudo(usuario)
            || s.CriadoPorLogin == usuario.Login
            || s.DestinatarioLogin == usuario.Login;
    }
}
