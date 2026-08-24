namespace AtualizadorVersaoRds;

/// <summary>
/// Alimenta o <see cref="ImageList"/> da lista de executaveis.
/// Os bitmaps ficam sob a guarda do chamador porque o ImageList so os copia quando o
/// handle nativo e criado - o que acontece na primeira exibicao do ListView. Liberar
/// um bitmap antes disso derruba a tela com "Parameter is not valid".
/// </summary>
internal static class ExeIcons
{
    /// <summary>
    /// Carrega um icone por caminho, na mesma ordem recebida, e registra os bitmaps em
    /// <paramref name="owned"/> para liberacao posterior.
    /// </summary>
    public static void Load(ImageList imageList, List<Bitmap> owned, IReadOnlyList<string> exePaths)
    {
        foreach (var exePath in exePaths)
        {
            var icon = LoadExeIcon(exePath);
            owned.Add(icon);
            imageList.Images.Add(icon);
        }
    }

    /// <summary>
    /// Esvazia o ImageList e libera os bitmaps que o alimentavam, evitando acumular
    /// handles GDI a cada recarga.
    /// </summary>
    public static void Clear(ImageList imageList, List<Bitmap> owned)
    {
        imageList.Images.Clear();

        foreach (var icon in owned)
        {
            icon.Dispose();
        }

        owned.Clear();
    }

    private static Bitmap LoadExeIcon(string exePath)
    {
        try
        {
            using var icon = Icon.ExtractAssociatedIcon(exePath);
            if (icon is not null)
            {
                return icon.ToBitmap();
            }
        }
        catch
        {
            // Fallback abaixo: caminho de rede, arquivo sem icone ou sem permissao.
        }

        return SystemIcons.Application.ToBitmap();
    }
}
