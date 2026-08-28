using System;
using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Foto de treino postada por um professor (ou admin), para o álbum público
    // do site. Regra: 1 foto por turma por AULA (ver DataAula) — repostar a mesma
    // (polo, turma, data) substitui a anterior. O binário fica no storage (Drive
    // ou banco); aqui guardamos só os metadados e o id do arquivo.
    public class FotoTreino : Base
    {
        // Categoria da foto no álbum público:
        //  "polo"       → treino de um polo (postado pelo professor; agrupa por polo)
        //  "graduacoes" | "geral" | "eventos" → coleções postadas pelo admin
        public string Categoria { get; set; } = "polo";
        public long PoloId { get; set; }
        public int Turma { get; set; }
        // Data da aula (só a data importa) — chave da trava 1/turma/aula e o que
        // o visitante do site vê em cada foto.
        public DateTime DataAula { get; set; }
        public string Legenda { get; set; } = string.Empty;
        // Identificador do arquivo no storage.
        public string ArquivoId { get; set; } = string.Empty;
        // Usuário que postou (professor ou admin).
        public long ProfessorId { get; set; }
        // Moderação: só entra no álbum público quando true. Pode já nascer true
        // se o polo estiver configurado para não exigir autorização.
        public bool Publicada { get; set; }
        public DateTime CriadoEm { get; set; }

        public override bool Validate()
        {
            _errors.Clear();

            // Polo e turma só valem para a categoria "polo" (treino do professor);
            // nas coleções do admin (graduações/geral/eventos) não há turma/polo.
            if (Categoria == "polo")
            {
                if (PoloId <= 0)
                    _errors.Add("A foto precisa estar vinculada a um polo.");
                if (Turma <= 0)
                    _errors.Add("A turma é obrigatória.");
            }

            if (DataAula == default)
                _errors.Add("A data da foto é obrigatória.");

            if (string.IsNullOrWhiteSpace(ArquivoId))
                _errors.Add("O arquivo da foto é obrigatório.");

            if (Legenda != null && Legenda.Length > 300)
                _errors.Add("A legenda deve ter no máximo 300 caracteres.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
