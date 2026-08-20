# ⚠️ DESCONTINUADO — app Flutter da chamada

> **Status: congelado (não receber mais funcionalidades).**
> Iniciado o desligamento em **2026-08-20**.

Este app Flutter foi o aplicativo de **chamada** das aulas de jiu-jitsu.
Ele está sendo **substituído pelo portal web em React como PWA**
(`TriboDeDaviWeb`), que agora também faz a chamada — instalável na tela
inicial do celular e com funcionamento **offline** no tatame.

## Por que foi descontinuado

- Consolidação num só front-end (React), eliminando a manutenção paralela de
  Flutter + React + Blazor.
- O PWA cobre o mesmo caso de uso (chamada no celular, offline) sem depender de
  publicação em loja de apps.
- A API C# (.NET) permanece a mesma — nada muda no backend.

## Não apague ainda

O código continua aqui **de propósito**, como rede de segurança, até que a
chamada em React seja validada em produção com professores reais.
O processo e os critérios para a remoção definitiva estão em
[`DESLIGAMENTO_FLUTTER.md`](../../DESLIGAMENTO_FLUTTER.md) (raiz do repositório).

## Se precisar rodar (temporariamente)

```bash
cd flutter_tribo_de_davi_api
flutter pub get
flutter run
```
