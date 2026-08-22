using System;

namespace InstitutoTriboDeDavi.Application.DTO
{
    public class LogAuditoriaDTO
    {
        public long Id { get; set; }
        public DateTime Data { get; set; }
        public string UsuarioLogin { get; set; }
        public string Acao { get; set; }
        public string Entidade { get; set; }
        public long EntidadeId { get; set; }
        public string Resumo { get; set; }
        public string Alteracoes { get; set; }
        public string Ip { get; set; }
    }
}
