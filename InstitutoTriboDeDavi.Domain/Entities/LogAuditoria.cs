using System;
using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Registro de quem alterou o quê e quando. Guardado para os dados que
    // exigem prestação de contas — financeiro, doações, presenças, cadastro de
    // alunos e usuários — onde "quem mudou isso?" precisa ter resposta.
    //
    // É gravado automaticamente pelo interceptor do EF (AuditoriaInterceptor),
    // não por chamadas espalhadas pelos serviços: assim nenhuma alteração
    // escapa por esquecimento de quem escreveu a funcionalidade.
    public class LogAuditoria : Base
    {
        public DateTime Data { get; set; }
        public string UsuarioLogin { get; set; } = string.Empty;
        // "Criou", "Alterou" ou "Excluiu".
        public string Acao { get; set; } = string.Empty;
        // Nome da entidade afetada (ex.: "Doacao", "Presenca").
        public string Entidade { get; set; } = string.Empty;
        public long EntidadeId { get; set; }
        // Resumo legível do que mudou, para a tela não precisar interpretar JSON.
        public string Resumo { get; set; } = string.Empty;
        // Apenas os campos alterados, em JSON: {"Valor":{"de":10,"para":20}}.
        public string Alteracoes { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;

        // O log é histórico: não passa por validação de domínio nem é editado.
        public override bool Validate() => true;
    }
}
