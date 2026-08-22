using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Services;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Infrastructure.Context;
using InstitutoTriboDeDavi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace InstitutoTriboDeDavi.Tests
{
    // Código de acesso do responsável: geração individual e a preparação em
    // lote para impressão.
    public class CodigoResponsavelTests : IDisposable
    {
        private readonly TriboDeDaviContext _context;
        private readonly AlunoService _service;

        public CodigoResponsavelTests()
        {
            var options = new DbContextOptionsBuilder<TriboDeDaviContext>()
                .UseInMemoryDatabase($"cod-{Guid.NewGuid()}")
                .Options;
            _context = new TriboDeDaviContext(options);

            var mapper = new MapperConfiguration(
                cfg => cfg.CreateMap<Aluno, AlunoDTO>().ReverseMap(),
                NullLoggerFactory.Instance).CreateMapper();

            _service = new AlunoService(mapper, new AlunoRepository(_context));
        }

        private Aluno Semear(string nome, string? codigo = null)
        {
            var aluno = new Aluno
            {
                Nome = nome,
                DataNascimento = new DateTime(2015, 1, 1),
                PoloId = 1,
                CodigoResponsavel = codigo,
            };
            _context.Alunos.Add(aluno);
            _context.SaveChanges();
            return aluno;
        }

        [Fact]
        public async Task Gerar_TornaOAlunoLocalizavelPeloCodigo()
        {
            var aluno = Semear("Ana");

            var codigo = await _service.GerarCodigoResponsavelAsync(aluno.Id);

            Assert.False(string.IsNullOrEmpty(codigo));
            var achado = await new AlunoRepository(_context).ObterPorCodigoResponsavelAsync(codigo);
            Assert.Equal(aluno.Id, achado.Id);
        }

        [Fact]
        public async Task Preparar_GeraOsFaltantes_ePreservaOsExistentes()
        {
            Semear("Ana", "JAEXISTE");
            Semear("Bruno"); // sem código

            var lista = await _service.PrepararCodigosResponsavelAsync(
                new UsuarioDTO { Role = UserRole.Administrador });

            Assert.Equal(2, lista.Count);
            // Todos passam a ter código...
            Assert.All(lista, i => Assert.False(string.IsNullOrEmpty(i.Codigo)));
            // ...e o que já tinha não é trocado.
            Assert.Contains(lista, i => i.Codigo == "JAEXISTE");
        }

        [Fact]
        public async Task Update_PreservaCodigoEAutorizacaoQuandoNaoEnviados()
        {
            var aluno = Semear("Ana Silva", "JAEXISTE");
            aluno.AutorizaImagem = true;
            _context.SaveChanges();

            // Formulário de edição não traz codigo nem autorizaImagem (null).
            await _service.Update(new AlunoDTO
            {
                Id = aluno.Id,
                Nome = "Ana Maria Silva",
                DataNascimento = aluno.DataNascimento,
                PoloId = 1,
                Turma = 2,
            });

            var salvo = await _context.Alunos.AsNoTracking().FirstAsync(a => a.Id == aluno.Id);
            Assert.Equal("Ana Maria Silva", salvo.Nome);          // atualizou o que veio
            Assert.Equal("JAEXISTE", salvo.CodigoResponsavel);     // preservou o código
            Assert.True(salvo.AutorizaImagem);                     // preservou a autorização
        }

        [Fact]
        public async Task Update_AtualizaAutorizacaoQuandoEnviada()
        {
            var aluno = Semear("Bruno Souza");
            aluno.AutorizaImagem = true;
            _context.SaveChanges();

            await _service.Update(new AlunoDTO
            {
                Id = aluno.Id,
                Nome = "Bruno Souza",
                DataNascimento = aluno.DataNascimento,
                PoloId = 1,
                Turma = 1,
                AutorizaImagem = false,
            });

            var salvo = await _context.Alunos.AsNoTracking().FirstAsync(a => a.Id == aluno.Id);
            Assert.False(salvo.AutorizaImagem);
        }

        public void Dispose() => _context.Dispose();
    }
}
