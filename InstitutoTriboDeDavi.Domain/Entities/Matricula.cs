using System;
using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Vínculo do aluno com o projeto em um ano. A filiação vale até 31/12, então
    // todo ano há rematrícula — é a matrícula, e não o cadastro do aluno, que
    // responde "quem está no projeto em 2026".
    public class Matricula : Base
    {
        public long AlunoId { get; set; }
        public int Ano { get; set; }
        public long PoloId { get; set; }
        public int Turma { get; set; }
        // Inscrição que originou a matrícula (nula quando lançada à mão).
        public long? InscricaoId { get; set; }
        public DateTime DataMatricula { get; set; }
        // Encerrada antes do fim do ano (desistência, transferência).
        public bool Ativa { get; set; } = true;
        public DateTime? DataEncerramento { get; set; }
        public string MotivoEncerramento { get; set; } = string.Empty;

        public override bool Validate()
        {
            _errors.Clear();

            if (AlunoId <= 0)
                _errors.Add("A matrícula precisa estar vinculada a um aluno.");
            if (Ano < 2000 || Ano > 2100)
                _errors.Add("O ano da matrícula é inválido.");
            if (PoloId <= 0)
                _errors.Add("A matrícula precisa de um polo.");
            if (Turma <= 0)
                _errors.Add("A matrícula precisa de uma turma.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
