namespace AtualizadorVersaoRds;

public static class UpdateService
{
    /// <summary>
    /// Atualiza cada executavel selecionado em cada pasta de servidor.
    ///
    /// Ordem por item (a cópia vem antes do backup de proposito):
    /// 1. copia a origem para um arquivo temporario no destino e confere o tamanho;
    /// 2. renomeia o executavel antigo para REMOVER_*;
    /// 3. promove o temporario para o nome final.
    ///
    /// Se qualquer etapa falha, as anteriores sao desfeitas: o executavel que estava
    /// no servidor nunca fica truncado nem some.
    /// </summary>
    public static UpdateSummary RunUpdate(
        string sourceFolder,
        IReadOnlyCollection<string> serverFolders,
        IReadOnlyCollection<string> selectedExeNames,
        IProgress<UpdateProgressInfo>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var summary = new UpdateSummary();
        var totalOperations = serverFolders.Count * selectedExeNames.Count;
        var completedOperations = 0;

        double CurrentPercent(double operationFraction = 0d)
        {
            if (totalOperations <= 0)
            {
                return 0d;
            }

            var value = (completedOperations + operationFraction) / totalOperations * 100d;
            return Math.Clamp(value, 0d, 100d);
        }

        void Report(string message, bool isError = false, bool appendToLog = true, double? percent = null)
        {
            progress?.Report(new UpdateProgressInfo
            {
                ProgressPercent = percent ?? CurrentPercent(),
                Message = message,
                IsError = isError,
                AppendToLog = appendToLog
            });
        }

        foreach (var serverFolder in serverFolders)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            Report($"Acessando pasta servidor {serverFolder}...");
            var serverExists = Directory.Exists(serverFolder);

            if (!serverExists)
            {
                Report($"ERRO: Pasta de servidor nao encontrada: {serverFolder}", true);
            }

            foreach (var exeName in selectedExeNames)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (!serverExists)
                {
                    Report($"[{serverFolder}] {exeName} ignorado (pasta de servidor indisponivel).", true);
                    summary.Failed++;
                    completedOperations++;
                    continue;
                }

                var succeeded = UpdateSingleFile(
                    sourceFolder,
                    serverFolder,
                    exeName,
                    Report,
                    fraction => CurrentPercent(fraction),
                    cancellationToken);

                if (succeeded)
                {
                    summary.Succeeded++;
                }
                else if (!cancellationToken.IsCancellationRequested)
                {
                    summary.Failed++;
                }

                completedOperations++;
                Report($"[{serverFolder}] Finalizado {exeName}.", appendToLog: false);
            }
        }

        if (cancellationToken.IsCancellationRequested)
        {
            summary.Cancelled = true;
            Report("Processo cancelado pelo usuario.", true);
            return summary;
        }

        Report("Processo finalizado.", appendToLog: false, percent: 100d);
        return summary;
    }

    private static bool UpdateSingleFile(
        string sourceFolder,
        string serverFolder,
        string exeName,
        Action<string, bool, bool, double?> report,
        Func<double, double> currentPercent,
        CancellationToken cancellationToken)
    {
        var sourceExe = Path.Combine(sourceFolder, exeName);
        var targetExe = Path.Combine(serverFolder, exeName);
        var removerBaseExe = Path.Combine(serverFolder, $"REMOVER_{exeName}");
        var tempExe = Path.Combine(serverFolder, $"{exeName}.novo.tmp");

        if (!File.Exists(sourceExe))
        {
            report($"[{serverFolder}] ERRO: Executavel de origem nao encontrado: {sourceExe}", true, true, null);
            return false;
        }

        // Etapa 1: copiar para um temporario e validar. Nada no servidor foi tocado ainda.
        report($"[{serverFolder}] Copiando {exeName}...", false, true, null);
        try
        {
            TryDelete(tempExe);
            CopyFileWithProgress(sourceExe, tempExe, copyPercent =>
            {
                report(
                    $"[{serverFolder}] Copiando {exeName}... {copyPercent:0}%",
                    false,
                    false,
                    currentPercent(copyPercent / 100d));
            }, cancellationToken);

            var expectedLength = new FileInfo(sourceExe).Length;
            var actualLength = new FileInfo(tempExe).Length;
            if (expectedLength != actualLength)
            {
                report(
                    $"[{serverFolder}] ERRO: Copia incompleta de {exeName} ({actualLength} de {expectedLength} bytes). Nada foi alterado.",
                    true,
                    true,
                    null);
                TryDelete(tempExe);
                return false;
            }
        }
        catch (OperationCanceledException)
        {
            TryDelete(tempExe);
            report($"[{serverFolder}] Copia de {exeName} cancelada. Nada foi alterado.", true, true, null);
            return false;
        }
        catch (Exception ex)
        {
            TryDelete(tempExe);
            report($"[{serverFolder}] ERRO ao copiar {exeName}: {ex.Message}. Nada foi alterado.", true, true, null);
            return false;
        }

        // Etapa 2: mover o executavel antigo para backup.
        string? backupExe = null;
        if (File.Exists(targetExe))
        {
            report($"[{serverFolder}] Renomeando {exeName} para REMOVER_{exeName}...", false, true, null);
            try
            {
                backupExe = GetNextAvailablePath(removerBaseExe);
                File.Move(targetExe, backupExe);
                report($"[{serverFolder}] Renomeado com sucesso: {targetExe} -> {backupExe}", false, true, null);
            }
            catch (Exception ex)
            {
                // Sem backup nao se sobrescreve nada: o executavel provavelmente esta em uso.
                TryDelete(tempExe);
                report(
                    $"[{serverFolder}] ERRO ao renomear {exeName}: {ex.Message}. Atualizacao deste arquivo cancelada (executavel preservado).",
                    true,
                    true,
                    null);
                return false;
            }
        }
        else
        {
            report($"[{serverFolder}] Executavel antigo nao encontrado: {targetExe}", false, true, null);
        }

        // Etapa 3: promover o temporario. Rename no mesmo volume e atomico.
        try
        {
            File.Move(tempExe, targetExe);
            report($"[{serverFolder}] Copia concluida: {sourceExe} -> {targetExe}", false, true, null);
            return true;
        }
        catch (Exception ex)
        {
            report($"[{serverFolder}] ERRO ao finalizar {exeName}: {ex.Message}", true, true, null);
            RestoreBackup(serverFolder, backupExe, targetExe, report);
            TryDelete(tempExe);
            return false;
        }
    }

    private static void RestoreBackup(
        string serverFolder,
        string? backupExe,
        string targetExe,
        Action<string, bool, bool, double?> report)
    {
        if (backupExe is null || !File.Exists(backupExe) || File.Exists(targetExe))
        {
            return;
        }

        try
        {
            File.Move(backupExe, targetExe);
            report($"[{serverFolder}] Versao anterior restaurada: {backupExe} -> {targetExe}", false, true, null);
        }
        catch (Exception ex)
        {
            report(
                $"[{serverFolder}] ATENCAO: nao foi possivel restaurar {backupExe}: {ex.Message}. Restaure manualmente.",
                true,
                true,
                null);
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Temporario orfao nao impede o resto do processo.
        }
    }

    internal static string GetNextAvailablePath(string basePath)
    {
        if (!File.Exists(basePath))
        {
            return basePath;
        }

        var directory = Path.GetDirectoryName(basePath) ?? string.Empty;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(basePath);
        var extension = Path.GetExtension(basePath);

        var counter = 2;
        while (true)
        {
            var candidateName = $"{fileNameWithoutExtension} ({counter}){extension}";
            var candidatePath = Path.Combine(directory, candidateName);
            if (!File.Exists(candidatePath))
            {
                return candidatePath;
            }

            counter++;
        }
    }

    private static void CopyFileWithProgress(
        string sourcePath,
        string targetPath,
        Action<double> onProgress,
        CancellationToken cancellationToken)
    {
        const int bufferSize = 1024 * 1024;
        var buffer = new byte[bufferSize];
        var fileLength = new FileInfo(sourcePath).Length;
        long totalRead = 0;
        var lastReportedPercent = -1;

        using var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);
        using (var targetStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize))
        {
            int bytesRead;
            while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                targetStream.Write(buffer, 0, bytesRead);
                totalRead += bytesRead;

                var percent = fileLength == 0 ? 100d : totalRead * 100d / fileLength;
                var rounded = (int)Math.Round(percent);
                if (rounded != lastReportedPercent || percent >= 100d)
                {
                    lastReportedPercent = rounded;
                    onProgress(Math.Clamp(percent, 0d, 100d));
                }
            }

            // Garante que os bytes chegaram ao disco antes da validacao de tamanho.
            targetStream.Flush(true);
        }
    }
}
