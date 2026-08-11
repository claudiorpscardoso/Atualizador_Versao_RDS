namespace AtualizadorVersaoRds;

/// <summary>
/// Caminhos gravaveis do aplicativo. Fica em %APPDATA% porque a pasta do executavel
/// pode ser somente-leitura (Program Files ou share de rede).
/// </summary>
public static class AppPaths
{
    public static string DataFolder { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "AtualizadorVersaoRds");

    public static string SettingsFile => Path.Combine(DataFolder, "settings.json");

    public static string LogFolder => Path.Combine(DataFolder, "logs");

    /// <summary>Local usado nas versoes 1.0.x, mantido apenas para migracao.</summary>
    public static string LegacySettingsFile => Path.Combine(AppContext.BaseDirectory, "settings.json");

    public static void EnsureDataFolder() => Directory.CreateDirectory(DataFolder);

    public static void EnsureLogFolder() => Directory.CreateDirectory(LogFolder);
}
