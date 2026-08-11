using System.Text.Json;

namespace AtualizadorVersaoRds;

public static class SettingsService
{
    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true
    };

    public static AppSettings Load()
    {
        try
        {
            var path = ResolveSettingsPath();
            if (path is null)
            {
                return new AppSettings();
            }

            var json = File.ReadAllText(path);
            var loaded = JsonSerializer.Deserialize<AppSettings>(json);

            if (loaded is null)
            {
                return new AppSettings();
            }

            loaded.SourceFolder ??= string.Empty;
            loaded.ServerFolders ??= [];
            loaded.ServerFolders = loaded.ServerFolders
                .Where(folder => !string.IsNullOrWhiteSpace(folder))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return loaded;
        }
        catch
        {
            return new AppSettings();
        }
    }

    /// <summary>
    /// Grava as configuracoes. Retorna false e a mensagem de erro em vez de lancar,
    /// para que a tela de configuracao possa avisar o usuario.
    /// </summary>
    public static bool TrySave(AppSettings settings, out string error)
    {
        try
        {
            AppPaths.EnsureDataFolder();
            var json = JsonSerializer.Serialize(settings, WriteOptions);
            File.WriteAllText(AppPaths.SettingsFile, json);
            error = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    /// <summary>
    /// Retorna o arquivo de configuracao a ser lido, migrando o arquivo antigo
    /// (ao lado do executavel) para %APPDATA% no primeiro uso. Null se nao existe nenhum.
    /// </summary>
    private static string? ResolveSettingsPath()
    {
        if (File.Exists(AppPaths.SettingsFile))
        {
            return AppPaths.SettingsFile;
        }

        if (!File.Exists(AppPaths.LegacySettingsFile))
        {
            return null;
        }

        try
        {
            AppPaths.EnsureDataFolder();
            File.Copy(AppPaths.LegacySettingsFile, AppPaths.SettingsFile);
            return AppPaths.SettingsFile;
        }
        catch
        {
            // Sem permissao para migrar: le direto do local antigo.
            return AppPaths.LegacySettingsFile;
        }
    }
}
