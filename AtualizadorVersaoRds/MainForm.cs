namespace AtualizadorVersaoRds;

public sealed class MainForm : Form
{
    private readonly Button _btnConfig = new();
    private readonly Button _btnReload = new();
    private readonly Button _btnToggleLog = new();
    private readonly Label _lblSourceInfo = new();
    private readonly Label _lblServersInfo = new();
    private readonly Label _lblProgress = new();
    private readonly Label _lblLog = new();
    private readonly ListView _exeListView = new();
    private readonly ImageList _exeImageList = new();
    private readonly ProgressBar _progressBar = new();
    private readonly TextBox _txtLog = new();
    private readonly Button _btnUpdate = new();
    private AppSettings _settings = new();

    public MainForm()
    {
        InitializeComponent();
        LoadSettingsAndExecutables();
    }

    private void InitializeComponent()
    {
        Text = "Atualizador de Versao RDS";
        Width = 900;
        Height = 700;
        StartPosition = FormStartPosition.CenterScreen;

        _btnConfig.Text = "Configuracoes";
        _btnConfig.Left = 20;
        _btnConfig.Top = 20;
        _btnConfig.Width = 130;
        _btnConfig.Click += (_, _) => OpenConfigForm();

        _btnReload.Text = "Recarregar EXEs";
        _btnReload.Left = 160;
        _btnReload.Top = 20;
        _btnReload.Width = 130;
        _btnReload.Click += (_, _) => LoadExecutableList();

        _btnToggleLog.Text = "Exibir log";
        _btnToggleLog.Left = 300;
        _btnToggleLog.Top = 20;
        _btnToggleLog.Width = 110;
        _btnToggleLog.Click += (_, _) => ToggleLog();

        _lblSourceInfo.Left = 20;
        _lblSourceInfo.Top = 60;
        _lblSourceInfo.Width = 840;
        _lblSourceInfo.Height = 40;
        _lblSourceInfo.AutoSize = true;

        _lblServersInfo.Left = 20;
        _lblServersInfo.Top = 95;
        _lblServersInfo.AutoSize = true;

        var lblExe = new Label
        {
            Text = "Executaveis disponiveis na origem:",
            Left = 20,
            Top = 140,
            Width = 280
        };

        _exeImageList.ImageSize = new Size(16, 16);
        _exeImageList.ColorDepth = ColorDepth.Depth32Bit;

        _exeListView.Left = 20;
        _exeListView.Top = 165;
        _exeListView.Width = 400;
        _exeListView.Height = 250;
        _exeListView.View = View.Details;
        _exeListView.CheckBoxes = true;
        _exeListView.FullRowSelect = true;
        _exeListView.GridLines = false;
        _exeListView.HeaderStyle = ColumnHeaderStyle.None;
        _exeListView.SmallImageList = _exeImageList;
        _exeListView.Columns.Add("Executavel", 380);

        _btnUpdate.Text = "Atualizar selecionados";
        _btnUpdate.Left = 20;
        _btnUpdate.Top = 430;
        _btnUpdate.Width = 180;
        _btnUpdate.Click += async (_, _) => await RunUpdateAsync();

        _progressBar.Left = 20;
        _progressBar.Top = 470;
        _progressBar.Width = 400;
        _progressBar.Height = 24;
        _progressBar.Minimum = 0;
        _progressBar.Maximum = 1000;
        _progressBar.Value = 0;

        _lblProgress.Left = 20;
        _lblProgress.Top = 500;
        _lblProgress.Width = 840;
        _lblProgress.Height = 40;
        _lblProgress.Text = "Pronto para executar.";

        _lblLog.Text = "Log de execucao:";
        _lblLog.Left = 440;
        _lblLog.Top = 140;
        _lblLog.Width = 200;
        _lblLog.Visible = false;

        _txtLog.Left = 440;
        _txtLog.Top = 165;
        _txtLog.Width = 420;
        _txtLog.Height = 350;
        _txtLog.Multiline = true;
        _txtLog.ScrollBars = ScrollBars.Vertical;
        _txtLog.ReadOnly = true;
        _txtLog.Visible = false;

        Controls.Add(_btnConfig);
        Controls.Add(_btnReload);
        Controls.Add(_btnToggleLog);
        Controls.Add(_lblSourceInfo);
        Controls.Add(_lblServersInfo);
        Controls.Add(lblExe);
        Controls.Add(_exeListView);
        Controls.Add(_btnUpdate);
        Controls.Add(_progressBar);
        Controls.Add(_lblProgress);
        Controls.Add(_lblLog);
        Controls.Add(_txtLog);
    }

    private void LoadSettingsAndExecutables()
    {
        _settings = SettingsService.Load();
        RefreshSettingsLabels();
        LoadExecutableList();
    }

    private void RefreshSettingsLabels()
    {
        _lblSourceInfo.Text = string.IsNullOrWhiteSpace(_settings.SourceFolder)
            ? "Origem: nao configurada"
            : $"Origem: {_settings.SourceFolder}";

        _lblServersInfo.Text = _settings.ServerFolders.Count == 0
            ? "Servidores: nenhum configurado"
            : $"Servidores configurados: {_settings.ServerFolders.Count}";
    }

    private void OpenConfigForm()
    {
        using var form = new ConfigForm(_settings);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            LoadSettingsAndExecutables();
            AppendLog("Configuracoes atualizadas.");
        }
    }

    private void LoadExecutableList()
    {
        _exeListView.Items.Clear();
        _exeImageList.Images.Clear();

        if (string.IsNullOrWhiteSpace(_settings.SourceFolder))
        {
            AppendLog("Configure a pasta de origem primeiro.");
            return;
        }

        if (!Directory.Exists(_settings.SourceFolder))
        {
            AppendLog($"Pasta de origem nao encontrada: {_settings.SourceFolder}");
            return;
        }

        var exeFiles = Directory.GetFiles(_settings.SourceFolder, "*.exe", SearchOption.TopDirectoryOnly)
            .OfType<string>()
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();

        for (var index = 0; index < exeFiles.Count; index++)
        {
            var exePath = exeFiles[index];
            var exeName = Path.GetFileName(exePath);

            _exeImageList.Images.Add(LoadExeIcon(exePath));
            var item = new ListViewItem(exeName, index)
            {
                Checked = false
            };

            _exeListView.Items.Add(item);
        }

        AppendLog($"{exeFiles.Count} executavel(is) encontrado(s) na origem.");
    }

    private async Task RunUpdateAsync()
    {
        if (string.IsNullOrWhiteSpace(_settings.SourceFolder) || !Directory.Exists(_settings.SourceFolder))
        {
            MessageBox.Show(this, "Pasta de origem invalida. Ajuste nas configuracoes.", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_settings.ServerFolders.Count == 0)
        {
            MessageBox.Show(this, "Nenhuma pasta de servidor configurada.", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedExeNames = _exeListView.CheckedItems
            .Cast<ListViewItem>()
            .Select(item => item.Text)
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToList();
        if (selectedExeNames.Count == 0)
        {
            MessageBox.Show(this, "Selecione ao menos um executavel.", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _btnUpdate.Enabled = false;
        _btnConfig.Enabled = false;
        _btnReload.Enabled = false;
        _exeListView.Enabled = false;
        UseWaitCursor = true;

        try
        {
            _progressBar.Minimum = 0;
            _progressBar.Maximum = 1000;
            _progressBar.Value = 0;
            _lblProgress.Text = "Iniciando atualizacao...";
            AppendLog("Inicio da atualizacao.");

            var progress = new Progress<UpdateProgressInfo>(info =>
            {
                var progressValue = (int)Math.Round(info.ProgressPercent * 10d);
                _progressBar.Value = Math.Clamp(progressValue, _progressBar.Minimum, _progressBar.Maximum);
                _lblProgress.Text = $"{info.Message} ({info.ProgressPercent:0.0}%)";
                if (info.AppendToLog)
                {
                    AppendLog(info.Message);
                }
            });

            await Task.Run(() =>
                UpdateService.RunUpdate(_settings.SourceFolder, _settings.ServerFolders, selectedExeNames, progress));

            _lblProgress.Text = "Atualizacao concluida.";
            _progressBar.Value = _progressBar.Maximum;
            AppendLog("Fim da atualizacao.");

            MessageBox.Show(this, "Atualizacao concluida. Veja o log para detalhes.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        finally
        {
            UseWaitCursor = false;
            _btnUpdate.Enabled = true;
            _btnConfig.Enabled = true;
            _btnReload.Enabled = true;
            _exeListView.Enabled = true;
        }
    }

    private void ToggleLog()
    {
        var show = !_txtLog.Visible;
        _txtLog.Visible = show;
        _lblLog.Visible = show;
        _btnToggleLog.Text = show ? "Ocultar log" : "Exibir log";
    }

    private void AppendLog(string message)
    {
        _txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
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
            // Fallback below.
        }

        return SystemIcons.Application.ToBitmap();
    }
}
