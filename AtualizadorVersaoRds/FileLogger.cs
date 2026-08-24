namespace AtualizadorVersaoRds;

/// <summary>
/// Log em arquivo diario. Uma ferramenta que altera producao precisa deixar rastro
/// que sobreviva ao fechamento da janela.
/// </summary>
public sealed class FileLogger
{
    private readonly object _sync = new();
    private readonly string? _filePath;

    public FileLogger()
    {
        try
        {
            AppPaths.EnsureLogFolder();
            _filePath = Path.Combine(AppPaths.LogFolder, $"atualizacao-{DateTime.Now:yyyyMMdd}.log");
        }
        catch
        {
            // Sem pasta de log o app continua funcionando, apenas sem persistir o log.
            _filePath = null;
        }
    }

    public string? FilePath => _filePath;

    public bool IsEnabled => _filePath is not null;

    public void Write(string message)
    {
        if (_filePath is null)
        {
            return;
        }

        try
        {
            lock (_sync)
            {
                File.AppendAllText(_filePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
        }
        catch
        {
            // Falha ao gravar log nao pode interromper a atualizacao.
        }
    }
}
