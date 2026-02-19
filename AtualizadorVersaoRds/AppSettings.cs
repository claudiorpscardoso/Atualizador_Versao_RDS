namespace AtualizadorVersaoRds;

public sealed class AppSettings
{
    public string SourceFolder { get; set; } = string.Empty;
    public List<string> ServerFolders { get; set; } = [];
}
