using InstitutoTriboDeDavi.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstitutoTriboDeDavi.Domain.Entities.Business
{
    public class SincronizacaoHistorico : Base
    {
        public DateTime DataExecucao { get; set; }
        public string PoloNome { get; set; }
        public long PoloId { get; set; }
        public int Inseridos { get; set; }
        public int Atualizados { get; set; }
        public int Ignorados { get; set; }
        public bool Sucesso { get; set; }
        public string Erros { get; set; } // JSON serializado
        public string Origem { get; set; } // "Automatico" ou "Manual"

        public override bool Validate() => true;
    }
}
