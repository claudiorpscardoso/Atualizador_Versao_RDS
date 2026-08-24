namespace AtualizadorVersaoRds.Tests;

public sealed class UpdateServiceTests
{
    [Fact]
    public void AtualizaExecutavelECriaBackup()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteSourceFile("App.exe", "versao-nova");
        workspace.WriteServerFile("App.exe", "versao-antiga");

        var summary = Run(workspace);

        Assert.Equal(1, summary.Succeeded);
        Assert.Equal(0, summary.Failed);
        Assert.False(summary.HasErrors);
        Assert.Equal("versao-nova", workspace.ReadServerFile("App.exe"));
        Assert.Equal("versao-antiga", workspace.ReadServerFile("REMOVER_App.exe"));
    }

    [Fact]
    public void CopiaMesmoQuandoNaoHaExecutavelAntigo()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteSourceFile("App.exe", "versao-nova");

        var summary = Run(workspace);

        Assert.Equal(1, summary.Succeeded);
        Assert.Equal("versao-nova", workspace.ReadServerFile("App.exe"));
        Assert.False(workspace.ServerFileExists("REMOVER_App.exe"));
    }

    [Fact]
    public void NaoSobrescreveBackupExistente()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteSourceFile("App.exe", "versao-nova");
        workspace.WriteServerFile("App.exe", "versao-2");
        workspace.WriteServerFile("REMOVER_App.exe", "versao-1");

        var summary = Run(workspace);

        Assert.Equal(1, summary.Succeeded);
        Assert.Equal("versao-1", workspace.ReadServerFile("REMOVER_App.exe"));
        Assert.Equal("versao-2", workspace.ReadServerFile("REMOVER_App (2).exe"));
        Assert.Equal("versao-nova", workspace.ReadServerFile("App.exe"));
    }

    [Fact]
    public void PreservaExecutavelQuandoRenomeacaoFalha()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteSourceFile("App.exe", "versao-nova");
        var targetPath = workspace.WriteServerFile("App.exe", "versao-antiga");

        // Segura o executavel de destino aberto, como faz um processo em execucao.
        using (new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var summary = Run(workspace);

            Assert.Equal(0, summary.Succeeded);
            Assert.Equal(1, summary.Failed);
            Assert.True(summary.HasErrors);
        }

        // O executavel original continua intacto e nenhum temporario ficou para tras.
        Assert.Equal("versao-antiga", workspace.ReadServerFile("App.exe"));
        Assert.Equal(new[] { "App.exe" }, workspace.ServerFiles());
    }

    [Fact]
    public void ReportaFalhaQuandoOrigemNaoExiste()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteServerFile("App.exe", "versao-antiga");

        var summary = Run(workspace);

        Assert.Equal(0, summary.Succeeded);
        Assert.Equal(1, summary.Failed);
        Assert.Equal("versao-antiga", workspace.ReadServerFile("App.exe"));
        Assert.Equal(new[] { "App.exe" }, workspace.ServerFiles());
    }

    [Fact]
    public void ReportaFalhaQuandoPastaDeServidorNaoExiste()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteSourceFile("App.exe", "versao-nova");
        var inexistente = Path.Combine(workspace.Root, "servidor-fantasma");

        var summary = UpdateService.RunUpdate(
            workspace.SourceFolder,
            new[] { inexistente },
            new[] { "App.exe" });

        Assert.Equal(0, summary.Succeeded);
        Assert.Equal(1, summary.Failed);
        Assert.False(Directory.Exists(inexistente));
    }

    [Fact]
    public void ContabilizaSucessoEFalhaNaMesmaExecucao()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteSourceFile("Bom.exe", "versao-nova");
        workspace.WriteServerFile("Bom.exe", "versao-antiga");

        var summary = UpdateService.RunUpdate(
            workspace.SourceFolder,
            new[] { workspace.ServerFolder },
            new[] { "Bom.exe", "Ausente.exe" });

        Assert.Equal(1, summary.Succeeded);
        Assert.Equal(1, summary.Failed);
        Assert.True(summary.HasErrors);
        Assert.Equal("versao-nova", workspace.ReadServerFile("Bom.exe"));
    }

    [Fact]
    public void CancelamentoAntesDeIniciarNaoAlteraNada()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteSourceFile("App.exe", "versao-nova");
        workspace.WriteServerFile("App.exe", "versao-antiga");

        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        var summary = UpdateService.RunUpdate(
            workspace.SourceFolder,
            new[] { workspace.ServerFolder },
            new[] { "App.exe" },
            cancellationToken: cancellation.Token);

        Assert.True(summary.Cancelled);
        Assert.Equal(0, summary.Succeeded);
        Assert.Equal("versao-antiga", workspace.ReadServerFile("App.exe"));
        Assert.Equal(new[] { "App.exe" }, workspace.ServerFiles());
    }

    [Fact]
    public void CancelamentoNoMeioParaAntesDosItensRestantes()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteSourceFile("A.exe", "nova-a");
        workspace.WriteSourceFile("B.exe", "nova-b");
        workspace.WriteServerFile("A.exe", "antiga-a");
        workspace.WriteServerFile("B.exe", "antiga-b");

        using var cancellation = new CancellationTokenSource();
        var progress = new Progress<UpdateProgressInfo>(info =>
        {
            if (info.Message.Contains("Copia concluida", StringComparison.OrdinalIgnoreCase))
            {
                cancellation.Cancel();
            }
        });

        var summary = UpdateService.RunUpdate(
            workspace.SourceFolder,
            new[] { workspace.ServerFolder },
            new[] { "A.exe", "B.exe" },
            progress,
            cancellation.Token);

        Assert.True(summary.Cancelled);
        Assert.Equal("nova-a", workspace.ReadServerFile("A.exe"));
        Assert.Equal("antiga-b", workspace.ReadServerFile("B.exe"));
    }

    [Fact]
    public void ProgressoTerminaEmCemPorCento()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteSourceFile("App.exe", "versao-nova");

        var reports = new List<UpdateProgressInfo>();
        var progress = new Progress<UpdateProgressInfo>(reports.Add);

        UpdateService.RunUpdate(
            workspace.SourceFolder,
            new[] { workspace.ServerFolder },
            new[] { "App.exe" },
            progress);

        Assert.NotEmpty(reports);
        Assert.All(reports, report => Assert.InRange(report.ProgressPercent, 0d, 100d));
        Assert.Equal(100d, reports[^1].ProgressPercent);
    }

    private static UpdateSummary Run(TempWorkspace workspace) => UpdateService.RunUpdate(
        workspace.SourceFolder,
        new[] { workspace.ServerFolder },
        new[] { "App.exe" });
}
