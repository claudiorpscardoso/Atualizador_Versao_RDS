namespace AtualizadorVersaoRds;

public static class UpdateService
{
    public static void RunUpdate(
        string sourceFolder,
        IReadOnlyCollection<string> serverFolders,
        IReadOnlyCollection<string> selectedExeNames,
        IProgress<UpdateProgressInfo>? progress = null)
    {
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
            Report($"Acessando pasta servidor {serverFolder}...");
            var serverExists = Directory.Exists(serverFolder);

            if (!serverExists)
            {
                Report($"ERRO: Pasta de servidor nao encontrada: {serverFolder}", true);
            }

            foreach (var exeName in selectedExeNames)
            {
                var sourceExe = Path.Combine(sourceFolder, exeName);
                var targetExe = Path.Combine(serverFolder, exeName);
                var removerBaseExe = Path.Combine(serverFolder, $"REMOVER_{exeName}");

                if (!serverExists)
                {
                    Report($"[{serverFolder}] Renomeando {exeName}... ignorado (pasta de servidor indisponivel).", true);
                    Report($"[{serverFolder}] Copiando {exeName}... ignorado (pasta de servidor indisponivel).", true);
                    completedOperations++;
                    continue;
                }

                Report($"[{serverFolder}] Renomeando {exeName} para REMOVER_{exeName}...");

                try
                {
                    if (File.Exists(targetExe))
                    {
                        var removerExe = GetNextAvailablePath(removerBaseExe);
                        File.Move(targetExe, removerExe);
                        Report($"[{serverFolder}] Renomeado com sucesso: {targetExe} -> {removerExe}");
                    }
                    else
                    {
                        Report($"[{serverFolder}] Executavel antigo nao encontrado: {targetExe}");
                    }
                }
                catch (Exception ex)
                {
                    Report($"[{serverFolder}] ERRO ao renomear {exeName}: {ex.Message}", true);
                }

                Report($"[{serverFolder}] Copiando {exeName}...");
                try
                {
                    if (!File.Exists(sourceExe))
                    {
                        Report($"[{serverFolder}] ERRO: Executavel de origem nao encontrado: {sourceExe}", true);
                    }
                    else
                    {
                        CopyFileWithProgress(sourceExe, targetExe, copyPercent =>
                        {
                            Report(
                                $"[{serverFolder}] Copiando {exeName}... {copyPercent:0}%",
                                appendToLog: false,
                                percent: CurrentPercent(copyPercent / 100d));
                        });
                        Report($"[{serverFolder}] Copia concluida: {sourceExe} -> {targetExe}");
                    }
                }
                catch (Exception ex)
                {
                    Report($"[{serverFolder}] ERRO ao copiar {exeName}: {ex.Message}", true);
                }

                completedOperations++;
                Report($"[{serverFolder}] Finalizado {exeName}.", appendToLog: false);
            }
        }

        Report("Processo finalizado.", appendToLog: false, percent: 100d);
    }

    private static string GetNextAvailablePath(string basePath)
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

    private static void CopyFileWithProgress(string sourcePath, string targetPath, Action<double> onProgress)
    {
        const int bufferSize = 1024 * 1024;
        var buffer = new byte[bufferSize];
        var fileLength = new FileInfo(sourcePath).Length;
        long totalRead = 0;
        var lastReportedPercent = -1;

        using var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);
        using var targetStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize);

        int bytesRead;
        while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
        {
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
    }
}
