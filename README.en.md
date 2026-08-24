# 🚀 User Guide - RDS Version Updater

[![Build Desktop App](https://github.com/claudiorpscardoso/Atualizador_Versao_RDS/actions/workflows/build.yml/badge.svg)](https://github.com/claudiorpscardoso/Atualizador_Versao_RDS/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Versão em português: [`README.md`](README.md)

## 📦 Quick start
Prerequisite: .NET 10 SDK installed.

```powershell
dotnet build
dotnet test
dotnet run --project .\AtualizadorVersaoRds
```

## 🔐 Security notice
Use it in a test environment first. Always validate source and destination folders before running in production.

## 🎯 What this app does
This app updates `.exe` files in one or more network server folders, using a configured source folder.

In short:
1. You set where executables are read from.
2. You set which server folders will be updated.
3. You choose which `.exe` files to update.
4. The app renames the old file to `REMOVER_*` and copies the new one.

---

## ⚙️ Update routine
For each server and each selected executable:
1. Access server folder.
2. Copy the new executable to a temporary file on the server and verify its size.
3. If the current executable exists, rename it to a `REMOVER_` backup.
4. Promote the temporary file to the final name.

### 🛡️ Why the copy happens before the backup
The server file is only touched after the copy completed and was validated.
If any step fails — network drop, file in use, missing permission — the process
rolls back and **the executable already on the server stays intact**. The item is
counted as a failure in the summary and the run moves on to the remaining items.

### 📦 Backup rule (important)
If `REMOVER_File.exe` already exists, the app **does not delete it**.
It creates a new name using Windows-style suffix:
- `REMOVER_File.exe`
- `REMOVER_File (2).exe`
- `REMOVER_File (3).exe`

✅ This keeps backup history.

---

## 🖥️ Main screen
On the main screen you have:
- `Configurações`: open settings screen.
- `Recarregar EXEs`: reload `.exe` list from source folder.
- `Exibir log`: show/hide execution log.
- Executable list with checkbox and icon.
- `Atualizar selecionados`: start update process (asks for confirmation first).
- `Cancelar`: safely stop a running update.
- Progress bar with real-time status.

---

## 🧩 First-time setup
1. Click `Configurações`.
2. Set the source folder with new `.exe` files.
3. Add one or more server destination folders.
4. Click `Salvar`.

💡 Tip: open network folders in Explorer first to confirm access.

---

## 🔄 Running an update
1. Click `Recarregar EXEs`.
2. Select the desired servers and executables.
3. Click `Atualizar selecionados` and confirm the summary shown.
4. Follow status and progress bar.
5. Check the completion message at the end — it reports how many items
   succeeded and how many failed.

💡 Right-click either list for `Marcar todos` / `Desmarcar todos`.

---

## ⏱️ During execution
While running:
- Executable list is locked to avoid changes mid-process.
- Main buttons are temporarily disabled.
- Status shows current server, file and action.
- Copy progress is shown as percentage.

---

## 🧾 Typical messages
- `Acessando pasta servidor ...`
- `[Server] Renomeando ...`
- `[Server] Copiando ... 45%`
- `[Server] Cópia concluída ...`
- `ERRO: Pasta de servidor não encontrada`
- `ERRO: Executável de origem não encontrado`

---

## 🛠️ Error handling
If an error occurs on one file/server:
- The app logs the issue.
- The process continues for remaining items.
- You can fix the issue and run again.

---

## ✅ Best practices
Before updating:
1. Confirm source folder.
2. Confirm configured server folders.
3. Confirm selected executables.

After updating:
1. Review log if needed.
2. Validate at least one server as sample.
3. If rollback is needed, use `REMOVER_*` backup files as reference.

---

## ❓ Quick FAQ
### Does it delete old backups?
No. It always creates a new `(N)` filename if a backup already exists.

### Can I update only some programs?
Yes. Select only the `.exe` files you want.

### Can I configure multiple servers?
Yes. Settings support multiple folder paths.

### Where are settings stored?
In `%APPDATA%\AtualizadorVersaoRds\settings.json`. Coming from 1.0.x, the old file
(next to the executable) is migrated automatically on first run.

### Where is the execution log?
In `%APPDATA%\AtualizadorVersaoRds\logs\atualizacao-YYYYMMDD.log`, one file per day.
The `Exibir log` panel shows the same content for the current session.

### Can I cancel mid-run?
Yes. `Cancelar` stops after the current file finishes, never leaving a
half-written executable on the server.

### Do I need .NET installed?
Depends on the package you download from the releases page:
- `...-win-x64.zip`: smaller, requires the **.NET Desktop Runtime 10**.
- `...-win-x64-self-contained.zip`: single executable, requires nothing installed.

---

## 📚 Useful project files
- `CHANGELOG.md`: version history.
- `LICENSE`: MIT license.
- `CONTRIBUTING.md`: quick contribution guide.

---

## 🤝 Contributing
Contributions are welcome.

Before opening a PR, please read:
- `CONTRIBUTING.md`
