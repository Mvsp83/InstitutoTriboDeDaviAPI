using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstitutoTriboDeDavi.System.DTO.Business
{
    public class PresencaDTO
    {
        public long Id { get; set; }
        public long AlunoId { get; set; }
        public string NomeAluno { get; set; }
        public long PoloId { get; set; }
        public DateTime Data { get; set; }
        public bool EstaPresente { get; set; }
        public string Observacoes { get; set; }
        public long AulaId { get; set; }
    }
}
