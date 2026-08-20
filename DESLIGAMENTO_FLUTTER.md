# Desligamento do app Flutter (chamada)

Plano de decomissionamento do app Flutter, substituído pelo portal **React
(TriboDeDaviWeb)** como PWA offline. Fase 4 da modernização.

- **Início:** 2026-08-20
- **Estado atual:** Flutter **congelado e descontinuado**, mas **mantido no
  repositório** como rede de segurança. Ainda **não** foi removido.
- **Backend:** inalterado — a API C# (.NET) serve os dois front-ends.

## Por que o código ainda está aqui

O Flutter é hoje a ferramenta que os professores usam de verdade. Só removemos
o código depois que a chamada em React for **validada em produção**. Remover
antes disso arriscaria deixar a ONG sem ferramenta de chamada.

## Critérios para a remoção definitiva (todos devem passar)

- [ ] Chamada em React testada ponta-a-ponta com a API em produção.
- [ ] Um professor real registrou presença de uma aula pendente (batch) com
      sucesso.
- [ ] Edição de uma aula já salva funcionando (correção registro a registro).
- [ ] Fluxo **offline** validado: marcar sem internet → reconectar → sincroniza
      (indicador na Topbar zera a fila).
- [ ] PWA instalado na tela inicial em **Android e iOS** (service worker
      registra num navegador real — o painel embutido não conta).
- [ ] Perfis conferidos: professor vê/edita só o seu polo; admin/supervisor só
      leem.
- [ ] Ao menos uma semana de uso real sem professores voltarem ao Flutter.

## Como remover quando os critérios forem atendidos

O código continua recuperável pelo histórico do git mesmo após a remoção.

```bash
# marca o último ponto com o Flutter, para referência futura
git tag flutter-final -m "Último estado do app Flutter antes da remoção"

# remove o app da árvore de trabalho (permanece no histórico e na tag)
git rm -r InstitutoTriboDeDavi.FLUTTER
git commit -m "Remove app Flutter (substituído pelo PWA React)"
```

Para recuperar depois, se necessário:

```bash
git checkout flutter-final -- InstitutoTriboDeDavi.FLUTTER
```

## Rollback (se o React apresentar problema em produção)

O app Flutter continua funcional. Enquanto a pasta existir, basta
`flutter run` / rebuild do APK a partir de
`InstitutoTriboDeDavi.FLUTTER/flutter_tribo_de_davi_api`.
