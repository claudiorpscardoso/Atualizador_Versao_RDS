using System.Windows.Forms;
using AtualizadorVersaoRds;

namespace AtualizadorVersaoRds.Tests;

/// <summary>
/// Regressao da v1.1.0: os bitmaps eram liberados logo apos entrar no ImageList e a
/// tela morria com ArgumentException ("Parameter is not valid") quando o ListView
/// criava o handle nativo.
/// </summary>
public sealed class ExeIconsTests
{
    [Fact]
    public void Load_MantemBitmapsValidosAteACriacaoDoHandleDoListView()
    {
        RunOnStaThread(() =>
        {
            using var workspace = new TempWorkspace();
            var exePaths = new[]
            {
                // Arquivo sem icone real: cai no fallback de SystemIcons.
                workspace.WriteSourceFile("Um.exe", "nao e um PE valido"),
                // Executavel de verdade: passa pelo ExtractAssociatedIcon.
                Environment.ProcessPath ?? workspace.WriteSourceFile("Dois.exe", "x"),
            };

            using var imageList = new ImageList { ImageSize = new Size(18, 18) };
            var owned = new List<Bitmap>();

            ExeIcons.Load(imageList, owned, exePaths);

            // Forca o mesmo caminho que quebrava (ImageList.CreateHandle ->
            // CreateBitmap -> Image.get_Width). Chamamos direto no ImageList, e nao via
            // ListView, para a falha estourar aqui em vez de virar caixa de dialogo do
            // WinForms dentro do WndProc.
            var handle = imageList.Handle;

            Assert.NotEqual(IntPtr.Zero, handle);
            Assert.Equal(exePaths.Length, imageList.Images.Count);
            Assert.Equal(exePaths.Length, owned.Count);
        });
    }

    [Fact]
    public void Clear_LiberaOsBitmapsEEsvaziaOImageList()
    {
        RunOnStaThread(() =>
        {
            using var workspace = new TempWorkspace();
            var exePaths = new[] { workspace.WriteSourceFile("Um.exe", "nao e um PE valido") };

            using var imageList = new ImageList { ImageSize = new Size(18, 18) };
            var owned = new List<Bitmap>();

            ExeIcons.Load(imageList, owned, exePaths);
            var icon = owned[0];

            ExeIcons.Clear(imageList, owned);

            Assert.Empty(owned);
            Assert.Empty(imageList.Images);
            Assert.Throws<ArgumentException>(() => icon.Width);
        });
    }

    /// <summary>Controles Windows Forms exigem apartamento STA.</summary>
    private static void RunOnStaThread(Action action)
    {
        Exception? failure = null;

        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                failure = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (failure is not null)
        {
            throw new Xunit.Sdk.XunitException($"Falhou na thread STA: {failure}");
        }
    }
}
