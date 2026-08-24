namespace AtualizadorVersaoRds.Tests;

/// <summary>
/// A regra de sufixo e o que garante que nenhum backup seja sobrescrito.
/// </summary>
public sealed class GetNextAvailablePathTests
{
    [Fact]
    public void RetornaCaminhoOriginalQuandoNaoExiste()
    {
        using var workspace = new TempWorkspace();
        var basePath = Path.Combine(workspace.ServerFolder, "REMOVER_App.exe");

        var result = UpdateService.GetNextAvailablePath(basePath);

        Assert.Equal(basePath, result);
    }

    [Fact]
    public void AdicionaSufixoDoisQuandoBaseJaExiste()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteServerFile("REMOVER_App.exe", "backup1");
        var basePath = Path.Combine(workspace.ServerFolder, "REMOVER_App.exe");

        var result = UpdateService.GetNextAvailablePath(basePath);

        Assert.Equal(Path.Combine(workspace.ServerFolder, "REMOVER_App (2).exe"), result);
    }

    [Fact]
    public void IncrementaAteEncontrarNomeLivre()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteServerFile("REMOVER_App.exe", "backup1");
        workspace.WriteServerFile("REMOVER_App (2).exe", "backup2");
        workspace.WriteServerFile("REMOVER_App (3).exe", "backup3");
        var basePath = Path.Combine(workspace.ServerFolder, "REMOVER_App.exe");

        var result = UpdateService.GetNextAvailablePath(basePath);

        Assert.Equal(Path.Combine(workspace.ServerFolder, "REMOVER_App (4).exe"), result);
    }

    [Fact]
    public void PreservaNomesComPontosNoMeio()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteServerFile("REMOVER_App.v2.exe", "backup1");
        var basePath = Path.Combine(workspace.ServerFolder, "REMOVER_App.v2.exe");

        var result = UpdateService.GetNextAvailablePath(basePath);

        Assert.Equal(Path.Combine(workspace.ServerFolder, "REMOVER_App.v2 (2).exe"), result);
    }
}
