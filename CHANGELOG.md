# Changelog

All notable changes to this project will be documented in this file.

## [1.1.1] - 2026-08-24
### Fixed
- A tela principal nao abre mais com o erro `Parameter is not valid` quando a pasta
  de origem tem executaveis. Ao corrigir o vazamento de handles GDI na 1.1.0 os
  bitmaps passaram a ser liberados antes de o `ImageList` copia-los, o que derrubava
  o aplicativo na criacao do `ListView`. Os bitmaps agora vivem enquanto a lista
  existe e sao liberados na recarga e no fechamento - sem reabrir o vazamento.
- Cobertura de teste para o caminho que falhava (`ExeIconsTests`).

## [1.1.0] - 2026-08-11
### Added
- Botão `Cancelar` para interromper uma atualização em andamento.
- Diálogo de confirmação antes de iniciar, com o resumo do que será alterado.
- Log gravado em arquivo diário em `%APPDATA%\AtualizadorVersaoRds\logs`.
- Resumo final com contagem de sucessos e falhas.
- Projeto de testes automatizados (`AtualizadorVersaoRds.Tests`).
- Release passa a publicar também um executável único self-contained.

### Changed
- **Cópia mais segura**: o novo executável é copiado para um arquivo temporário e
  validado por tamanho antes de substituir o arquivo do servidor.
- `settings.json` passou para `%APPDATA%\AtualizadorVersaoRds` (migração automática
  do arquivo antigo), permitindo instalação em pastas somente-leitura.
- A mensagem final agora diferencia sucesso, falhas e cancelamento.

### Fixed
- Executável do servidor não é mais truncado quando o backup `REMOVER_*` falha
  (ex.: arquivo em uso). O item é abortado e a versão anterior é preservada.
- Erro inesperado durante a atualização não derruba mais o aplicativo.
- Falha ao salvar configurações agora exibe mensagem em vez de encerrar o app.
- Vazamento de handles GDI ao recarregar a lista de executáveis.

## [1.0.0] - 2026-02-20
### Added
- Windows app (WinForms) for updating `.exe` files to multiple server folders.
- Configuration screen with source folder and multiple server paths.
- Main screen with executable selection (checkbox + icon).
- Real-time progress bar and status updates during copy.
- Optional execution log panel.
- Safe backup rename strategy using `REMOVER_*` with incremental suffix `(N)`.
- User-focused README for operation guidance.
