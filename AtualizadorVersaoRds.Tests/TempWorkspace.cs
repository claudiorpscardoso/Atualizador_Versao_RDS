namespace AtualizadorVersaoRds.Tests;

/// <summary>
/// Pasta temporaria descartavel com atalhos para montar cenarios de origem/servidor.
/// </summary>
public sealed class TempWorkspace : IDisposable
{
    public TempWorkspace()
    {
        Root = Path.Combine(Path.GetTempPath(), "AtualizadorRdsTests", Guid.NewGuid().ToString("N"));
        SourceFolder = Path.Combine(Root, "origem");
        ServerFolder = Path.Combine(Root, "servidor");
        Directory.CreateDirectory(SourceFolder);
        Directory.CreateDirectory(ServerFolder);
    }

    public string Root { get; }

    public string SourceFolder { get; }

    public string ServerFolder { get; }

    public string WriteSourceFile(string name, string content)
    {
        var path = Path.Combine(SourceFolder, name);
        File.WriteAllText(path, content);
        return path;
    }

    public string WriteServerFile(string name, string content)
    {
        var path = Path.Combine(ServerFolder, name);
        File.WriteAllText(path, content);
        return path;
    }

    public string ReadServerFile(string name) => File.ReadAllText(Path.Combine(ServerFolder, name));

    public bool ServerFileExists(string name) => File.Exists(Path.Combine(ServerFolder, name));

    public string[] ServerFiles() => Directory
        .GetFiles(ServerFolder)
        .Select(Path.GetFileName)
        .OfType<string>()
        .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    public void Dispose()
    {
        try
        {
            Directory.Delete(Root, recursive: true);
        }
        catch
        {
            // Limpeza best-effort: arquivo travado nao deve derrubar o teste.
        }
    }
}
