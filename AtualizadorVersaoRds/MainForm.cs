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

    private readonly Panel _headerPanel = new();
    private readonly FlowLayoutPanel _headerButtonsPanel = new();
    private readonly Panel _leftCard = new();
    private readonly Panel _rightCard = new();

    private readonly Font _titleFont = new("Segoe UI Semibold", 16f, FontStyle.Bold);
    private readonly Font _subtitleFont = new("Segoe UI", 9.5f, FontStyle.Regular);
    private readonly Font _sectionFont = new("Segoe UI Semibold", 10.5f, FontStyle.Bold);

    private AppSettings _settings = new();

    public MainForm()
    {
        InitializeComponent();
        LoadSettingsAndExecutables();
    }

    private void InitializeComponent()
    {
        Text = "Atualizador de Versao RDS";
        Width = 1040;
        Height = 720;
        MinimumSize = new Size(980, 660);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(241, 245, 249);

        _headerPanel.Left = 16;
        _headerPanel.Top = 12;
        _headerPanel.Width = ClientSize.Width - 32;
        _headerPanel.Height = 78;
        _headerPanel.BackColor = Color.FromArgb(15, 23, 42);
        _headerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        var lblTitle = new Label
        {
            Text = "Atualizador RDS",
            Left = 18,
            Top = 14,
            AutoSize = true,
            ForeColor = Color.White,
            Font = _titleFont,
            BackColor = Color.Transparent
        };

        var lblSubtitle = new Label
        {
            Text = "Selecione os executaveis e atualize os servidores com seguranca",
            Left = 20,
            Top = 47,
            AutoSize = true,
            ForeColor = Color.FromArgb(203, 213, 225),
            Font = _subtitleFont,
            BackColor = Color.Transparent
        };

        ConfigureActionButton(_btnConfig, "Configuracoes", 0, 0, 120, false);
        _btnConfig.Click += (_, _) => OpenConfigForm();

        ConfigureActionButton(_btnReload, "Recarregar EXEs", 0, 0, 122, false);
        _btnReload.Click += (_, _) => LoadExecutableList();

        ConfigureActionButton(_btnToggleLog, "Exibir log", 0, 0, 110, false);
        _btnToggleLog.Click += (_, _) => ToggleLog();

        _headerButtonsPanel.Left = 0;
        _headerButtonsPanel.Top = 22;
        _headerButtonsPanel.Width = _headerPanel.Width - 20;
        _headerButtonsPanel.Height = 34;
        _headerButtonsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _headerButtonsPanel.FlowDirection = FlowDirection.RightToLeft;
        _headerButtonsPanel.WrapContents = false;
        _headerButtonsPanel.BackColor = Color.Transparent;
        _headerButtonsPanel.Padding = new Padding(0);
        _headerButtonsPanel.Margin = new Padding(0);

        _headerButtonsPanel.Controls.Add(_btnToggleLog);
        _headerButtonsPanel.Controls.Add(_btnReload);
        _headerButtonsPanel.Controls.Add(_btnConfig);

        _headerPanel.Controls.Add(lblTitle);
        _headerPanel.Controls.Add(lblSubtitle);
        _headerPanel.Controls.Add(_headerButtonsPanel);

        var infoPanel = new Panel
        {
            Left = 16,
            Top = 98,
            Width = ClientSize.Width - 32,
            Height = 70,
            BackColor = Color.White,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        _lblSourceInfo.Left = 14;
        _lblSourceInfo.Top = 12;
        _lblSourceInfo.Width = infoPanel.Width - 28;
        _lblSourceInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _lblSourceInfo.Font = _subtitleFont;
        _lblSourceInfo.ForeColor = Color.FromArgb(30, 41, 59);

        _lblServersInfo.Left = 14;
        _lblServersInfo.Top = 36;
        _lblServersInfo.Width = infoPanel.Width - 28;
        _lblServersInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _lblServersInfo.Font = _subtitleFont;
        _lblServersInfo.ForeColor = Color.FromArgb(30, 41, 59);

        infoPanel.Controls.Add(_lblSourceInfo);
        infoPanel.Controls.Add(_lblServersInfo);

        _leftCard.Left = 16;
        _leftCard.Top = 178;
        _leftCard.Width = 620;
        _leftCard.Height = 390;
        _leftCard.BackColor = Color.White;
        _leftCard.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;

        var lblExe = new Label
        {
            Text = "Executaveis disponiveis",
            Left = 14,
            Top = 12,
            AutoSize = true,
            Font = _sectionFont,
            ForeColor = Color.FromArgb(15, 23, 42)
        };

        _exeImageList.ImageSize = new Size(18, 18);
        _exeImageList.ColorDepth = ColorDepth.Depth32Bit;

        _exeListView.Left = 14;
        _exeListView.Top = 42;
        _exeListView.Width = _leftCard.Width - 28;
        _exeListView.Height = _leftCard.Height - 56;
        _exeListView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        _exeListView.View = View.Details;
        _exeListView.CheckBoxes = true;
        _exeListView.FullRowSelect = true;
        _exeListView.GridLines = false;
        _exeListView.HeaderStyle = ColumnHeaderStyle.None;
        _exeListView.BorderStyle = BorderStyle.FixedSingle;
        _exeListView.SmallImageList = _exeImageList;
        _exeListView.BackColor = Color.FromArgb(248, 250, 252);
        _exeListView.Columns.Add("Executavel", _exeListView.Width - 26);

        _leftCard.Controls.Add(lblExe);
        _leftCard.Controls.Add(_exeListView);

        _rightCard.Left = 646;
        _rightCard.Top = 178;
        _rightCard.Width = 378;
        _rightCard.Height = 390;
        _rightCard.BackColor = Color.White;
        _rightCard.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
        _rightCard.Visible = false;

        _lblLog.Text = "Log de execucao";
        _lblLog.Left = 14;
        _lblLog.Top = 12;
        _lblLog.AutoSize = true;
        _lblLog.Font = _sectionFont;
        _lblLog.ForeColor = Color.FromArgb(15, 23, 42);

        _txtLog.Left = 14;
        _txtLog.Top = 42;
        _txtLog.Width = _rightCard.Width - 28;
        _txtLog.Height = _rightCard.Height - 56;
        _txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        _txtLog.Multiline = true;
        _txtLog.ScrollBars = ScrollBars.Vertical;
        _txtLog.ReadOnly = true;
        _txtLog.BorderStyle = BorderStyle.FixedSingle;
        _txtLog.BackColor = Color.FromArgb(248, 250, 252);
        _txtLog.Font = new Font("Consolas", 9f, FontStyle.Regular);

        _rightCard.Controls.Add(_lblLog);
        _rightCard.Controls.Add(_txtLog);

        ConfigureActionButton(_btnUpdate, "Atualizar selecionados", 16, 582, 220, true);
        _btnUpdate.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
        _btnUpdate.Click += async (_, _) => await RunUpdateAsync();

        _progressBar.Left = 252;
        _progressBar.Top = 586;
        _progressBar.Width = ClientSize.Width - 268;
        _progressBar.Height = 22;
        _progressBar.Minimum = 0;
        _progressBar.Maximum = 1000;
        _progressBar.Value = 0;
        _progressBar.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

        _lblProgress.Left = 252;
        _lblProgress.Top = 612;
        _lblProgress.Width = ClientSize.Width - 268;
        _lblProgress.Height = 25;
        _lblProgress.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        _lblProgress.Font = _subtitleFont;
        _lblProgress.ForeColor = Color.FromArgb(51, 65, 85);
        _lblProgress.Text = "Pronto para executar.";

        Controls.Add(_headerPanel);
        Controls.Add(infoPanel);
        Controls.Add(_leftCard);
        Controls.Add(_rightCard);
        Controls.Add(_btnUpdate);
        Controls.Add(_progressBar);
        Controls.Add(_lblProgress);

        Resize += (_, _) => AdjustLayout();
        AdjustLayout();
    }

    private void ConfigureActionButton(Button button, string text, int left, int top, int width, bool primary)
    {
        button.Text = text;
        button.Left = left;
        button.Top = top;
        button.Width = width;
        button.Height = 34;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Cursor = Cursors.Hand;
        button.Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);
        button.BackColor = primary ? Color.FromArgb(14, 116, 144) : Color.FromArgb(226, 232, 240);
        button.ForeColor = primary ? Color.White : Color.FromArgb(15, 23, 42);
        button.Margin = new Padding(6, 0, 0, 0);
    }

    private void AdjustLayout()
    {
        var availableWidth = ClientSize.Width - 32;
        var showLog = _rightCard.Visible;
        var buttonsWidth = _headerButtonsPanel.Controls.Cast<Control>().Sum(control => control.Width + control.Margin.Left + control.Margin.Right);
        _headerButtonsPanel.Width = buttonsWidth;
        _headerButtonsPanel.Left = Math.Max(14, _headerPanel.Width - _headerButtonsPanel.Width - 14);

        if (showLog)
        {
            _leftCard.Width = Math.Max(460, (availableWidth - 10) * 60 / 100);
            _rightCard.Left = _leftCard.Right + 10;
            _rightCard.Width = availableWidth - _leftCard.Width - 10;
        }
        else
        {
            _leftCard.Width = availableWidth;
        }

        _exeListView.Columns[0].Width = Math.Max(220, _exeListView.ClientSize.Width - 4);
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
        _btnToggleLog.Enabled = false;
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
            _btnToggleLog.Enabled = true;
            _exeListView.Enabled = true;
        }
    }

    private void ToggleLog()
    {
        var show = !_rightCard.Visible;
        _rightCard.Visible = show;
        _btnToggleLog.Text = show ? "Ocultar log" : "Exibir log";
        AdjustLayout();
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
