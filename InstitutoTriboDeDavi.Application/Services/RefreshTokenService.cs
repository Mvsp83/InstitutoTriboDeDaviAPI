using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _repository;

        // Sessão longa o bastante para o uso no tatame sem re-login frequente,
        // mas finita — some depois de 30 dias sem renovar.
        private const int DiasValidade = 30;

        public RefreshTokenService(IRefreshTokenRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> EmitirAsync(long usuarioId)
        {
            var raw = GerarTokenAleatorio();

            await _repository.CriarAsync(new RefreshToken
            {
                UsuarioId = usuarioId,
                TokenHash = Hash(raw),
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow.AddDays(DiasValidade),
            });

            return raw;
        }

        public async Task<RotacaoRefresh> RotacionarAsync(string rawToken)
        {
            if (string.IsNullOrWhiteSpace(rawToken))
                return null;

            var token = await _repository.ObterPorHashAsync(Hash(rawToken));
            if (token == null || !token.Ativo)
                return null;

            // Rotação: o token usado morre e um novo nasce. Reuso de um token já
            // rotacionado (possível roubo) cai no !Ativo e é recusado.
            await _repository.RevogarAsync(token);
            var novo = await EmitirAsync(token.UsuarioId);

            return new RotacaoRefresh(token.UsuarioId, novo);
        }

        public async Task RevogarAsync(string rawToken)
        {
            if (string.IsNullOrWhiteSpace(rawToken))
                return;

            var token = await _repository.ObterPorHashAsync(Hash(rawToken));
            if (token != null && token.RevogadoEm == null)
                await _repository.RevogarAsync(token);
        }

        public Task<int> RevogarUsuarioAsync(long usuarioId) =>
            _repository.RevogarTodosDoUsuarioAsync(usuarioId);

        // 256 bits de entropia; base64 cabe num JSON/localStorage sem problema.
        private static string GerarTokenAleatorio() =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        // Guardamos só o hash: um vazamento do banco não devolve tokens usáveis.
        private static string Hash(string raw) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
    }
}
