using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Tests
{
    // Testes da validação de domínio ativada no item 3:
    // garante que Validate() coleta TODOS os erros (não só o primeiro)
    // e que entidades válidas passam sem exceção.
    public class ValidacaoEntidadesTests
    {
        [Fact]
        public void Usuario_Valido_RetornaTrue()
        {
            var usuario = new Usuario
            {
                Login = "professor1",
                Email = "professor@teste.com",
                SenhaHash = "hash-qualquer-de-84-caracteres",
                Role = UserRole.Professor
            };

            Assert.True(usuario.Validate());
        }

        [Fact]
        public void Usuario_Invalido_LancaDomainExceptionComTodosOsErros()
        {
            var usuario = new Usuario
            {
                Login = "ab",              // menor que o mínimo de 3
                Email = "email-invalido",  // não passa no regex
                SenhaHash = ""             // vazio
            };

            var ex = Assert.Throws<DomainException>(() => usuario.Validate());

            // Deve coletar os erros das três regras, não parar no primeiro
            Assert.True(ex.Errors.Count >= 3);
        }

        [Fact]
        public void Aluno_NomeCurto_LancaDomainException()
        {
            var aluno = new Aluno { Nome = "ab" };

            Assert.Throws<DomainException>(() => aluno.Validate());
        }

        [Fact]
        public void Aula_HoraFimMenorQueInicio_LancaDomainException()
        {
            var aula = new Aula
            {
                PoloId = 1,
                Data = DateTime.Today,
                HoraInicio = new TimeSpan(10, 0, 0),
                HoraFim = new TimeSpan(9, 0, 0),
                Turma = 1
            };

            Assert.Throws<DomainException>(() => aula.Validate());
        }

        [Fact]
        public void Aula_Valida_RetornaTrue()
        {
            var aula = new Aula
            {
                PoloId = 1,
                Data = DateTime.Today,
                HoraInicio = new TimeSpan(9, 0, 0),
                HoraFim = new TimeSpan(10, 0, 0),
                Turma = 1
            };

            Assert.True(aula.Validate());
        }

        [Fact]
        public void Presenca_SemPolo_LancaDomainException()
        {
            var presenca = new Presenca
            {
                AlunoId = 1,
                AulaId = 1,
                PoloId = 0, // inválido
                Data = DateTime.Today
            };

            Assert.Throws<DomainException>(() => presenca.Validate());
        }
    }
}
