# Política de Retenção e Eliminação de Dados (LGPD)

Instituto Tribo de Davi — tratamento de dados pessoais de alunos (menores de
idade) e responsáveis. Referência: Lei 13.709/2018 (LGPD).

> Este documento descreve **quanto tempo** cada dado é guardado, **com que base
> legal**, e **como** os direitos do titular (acesso, portabilidade e
> eliminação) são atendidos no sistema. Não substitui orientação jurídica.

## 1. Bases legais

| Tratamento | Base legal (LGPD) |
|---|---|
| Cadastro e frequência do aluno | Execução de política pública / legítimo interesse do projeto socioeducativo, com **consentimento do responsável** (art. 7, 14) |
| Uso de imagem (fotos/vídeos) | Consentimento específico do responsável (aceite próprio na ficha) |
| Prestação de contas a editais e órgãos | Cumprimento de obrigação legal/regulatória (art. 7, II) |
| Questionário de aptidão física (PAR-Q) | Proteção da vida/incolumidade do menor (art. 11) — dado sensível de saúde |

O consentimento é coletado na **ficha de inscrição online**, que registra os
aceites, o **nome de quem assinou**, a **versão dos termos** e a **data** — essa
evidência é preservada mesmo após a eliminação dos demais dados (art. 37).

## 2. Categorias e prazos de retenção

| Dado | Retenção | Após o prazo |
|---|---|---|
| Cadastro do aluno (nome, RG, CPF, endereço, responsável) | Enquanto matriculado + **18 meses** de inatividade | Anonimização |
| Presença / frequência | Enquanto o aluno existir (anonimizado após eliminação) | Mantido anonimizado (estatística/editais) |
| Matrícula e graduação | Permanente (histórico/accountability) | Mantido anonimizado |
| Ficha de inscrição (PII) | Anonimizada junto com o aluno | Só aceites/versão/data ficam |
| Log de auditoria | Permanente; **nunca guarda valores de PII**, só o fato da alteração | — |

"Inatividade" = sem presença registrada e sem matrícula vigente. O prazo padrão
de 18 meses cobre um ciclo anual completo mais margem para rematrícula.

## 3. Direitos do titular — como atender

Todos exigem perfil **Administrador**. Operam sobre um aluno pelo `id`.

### Acesso / portabilidade (art. 18, II e V)
`GET /api/Aluno/{id}/exportar-dados` → devolve, num único JSON, o cadastro e
todo o histórico (matrículas, graduações, presenças, inscrições). É o pacote a
entregar ao responsável que solicitar seus dados.

### Eliminação (art. 18, VI)
`POST /api/Aluno/{id}/anonimizar` → apaga os **identificadores diretos** do
aluno (nome, RG, CPF, endereço, contatos, responsável) e da inscrição vinculada,
e o nome copiado nas presenças. Preserva, anonimizados, os registros que
sustentam a prestação de contas (presença, matrícula, graduação) e a **prova de
consentimento** da inscrição. A operação é **idempotente** e registrada na
auditoria como alteração do aluno, sem expor os valores apagados.

> Optou-se por **anonimizar** em vez de apagar tudo porque a LGPD (art. 16)
> autoriza reter dados para cumprimento de obrigação legal e prestação de
> contas, desde que os dados pessoais sejam eliminados. Uma exclusão física
> total continua disponível pelo endpoint administrativo `DELETE
> /api/Aluno/delete/{id}` quando não houver dever de retenção.

## 4. Rotina de retenção

`GET /api/Aluno/candidatos-retencao?mesesInativo=18` lista os alunos que já
passaram do prazo de inatividade — candidatos à anonimização. Recomenda-se rodar
a varredura **uma vez por ano** (ex.: início do ano letivo, após a janela de
rematrícula) e anonimizar os que não retornaram.

## 5. Minimização e segurança

- O log de auditoria **não armazena valores de dados pessoais** — apenas que um
  campo mudou, quem alterou e quando.
- Senhas e hashes nunca entram em log.
- Acesso a exportação/eliminação restrito a Administrador.
