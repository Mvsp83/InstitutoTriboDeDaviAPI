# Desligamento do app Flutter (chamada) — concluído

O app **Flutter** (chamada usada pelos professores) foi substituído pelo portal
**React (TriboDeDaviWeb)** como PWA offline e **removido do repositório em
2026-09-10**. O backend (API C#/.NET) atende só o portal React agora.

- **Início do plano:** 2026-08-20
- **Remoção:** 2026-09-10

## Como recuperar (se precisar)

O código continua no histórico do git, marcado pela tag **`flutter-final`**:

```bash
# ver o app como estava antes da remoção
git checkout flutter-final -- InstitutoTriboDeDavi.FLUTTER
```

A partir daí dá para `flutter run` / rebuild do APK em
`InstitutoTriboDeDavi.FLUTTER/flutter_tribo_de_davi_api`, caso o PWA React
precise de um rollback temporário.
