# 🚀 User Guide - RDS Version Updater

Versão em português: [`README.md`](README.md)

## 📦 Quick start
Prerequisite: .NET 10 SDK installed.

```powershell
dotnet build
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
2. Find the current executable in that folder.
3. If found, rename it to a `REMOVER_` backup.
4. Copy the new executable from source folder.

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
- `Atualizar selecionados`: start update process.
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
2. Select desired executables.
3. Click `Atualizar selecionados`.
4. Follow status and progress bar.
5. Check the completion message at the end.

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
In `settings.json`, next to the app executable.

---

## 📚 Useful project files
- `CHANGELOG.md`: version history.
- `LICENSE`: MIT license.
