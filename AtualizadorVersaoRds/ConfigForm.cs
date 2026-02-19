namespace AtualizadorVersaoRds;

public sealed class ConfigForm : Form
{
    private readonly TextBox _txtSourceFolder = new();
    private readonly TextBox _txtServerFolder = new();
    private readonly ListBox _lstServers = new();
    private readonly Button _btnSave = new();
    private readonly Button _btnCancel = new();
    private readonly Button _btnSourceBrowse = new();
    private readonly Button _btnServerBrowse = new();
    private readonly Button _btnAddServer = new();
    private readonly Button _btnRemoveServer = new();

    private AppSettings _settings;

    public ConfigForm(AppSettings settings)
    {
        _settings = settings;
        InitializeComponent();
        LoadFromSettings();
    }

    private void InitializeComponent()
    {
        Text = "Configuracoes";
        Width = 820;
        Height = 570;
        MinimumSize = new Size(780, 520);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.FromArgb(241, 245, 249);

        var lblTitle = new Label
        {
            Text = "Configuracoes de Atualizacao",
            Left = 20,
            Top = 18,
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 15f, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42)
        };

        var lblSubtitle = new Label
        {
            Text = "Defina a pasta de origem e os servidores que receberao os executaveis",
            Left = 22,
            Top = 48,
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(71, 85, 105)
        };

        var sourceCard = new Panel
        {
            Left = 20,
            Top = 80,
            Width = 760,
            Height = 95,
            BackColor = Color.White
        };

        var lblSource = new Label
        {
            Text = "Pasta de origem dos executaveis",
            Left = 14,
            Top = 14,
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 10f, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42)
        };

        _txtSourceFolder.Left = 14;
        _txtSourceFolder.Top = 42;
        _txtSourceFolder.Width = 620;
        _txtSourceFolder.Height = 30;
        _txtSourceFolder.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

        ConfigureSecondaryButton(_btnSourceBrowse, "Buscar...", 640, 41, 105);
        _btnSourceBrowse.Click += (_, _) => SelectSourceFolder();

        sourceCard.Controls.Add(lblSource);
        sourceCard.Controls.Add(_txtSourceFolder);
        sourceCard.Controls.Add(_btnSourceBrowse);

        var serverCard = new Panel
        {
            Left = 20,
            Top = 186,
            Width = 760,
            Height = 300,
            BackColor = Color.White
        };

        var lblServers = new Label
        {
            Text = "Pastas de servidores",
            Left = 14,
            Top = 14,
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 10f, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42)
        };

        _txtServerFolder.Left = 14;
        _txtServerFolder.Top = 42;
        _txtServerFolder.Width = 500;
        _txtServerFolder.Height = 30;
        _txtServerFolder.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

        ConfigureSecondaryButton(_btnServerBrowse, "Buscar...", 520, 41, 100);
        _btnServerBrowse.Click += (_, _) => SelectServerFolder();

        ConfigurePrimaryButton(_btnAddServer, "Adicionar", 626, 41, 119);
        _btnAddServer.Click += (_, _) => AddServerFolder();

        _lstServers.Left = 14;
        _lstServers.Top = 84;
        _lstServers.Width = 606;
        _lstServers.Height = 190;
        _lstServers.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
        _lstServers.BackColor = Color.FromArgb(248, 250, 252);

        ConfigureSecondaryButton(_btnRemoveServer, "Remover", 626, 84, 119);
        _btnRemoveServer.Click += (_, _) => RemoveSelectedServer();

        serverCard.Controls.Add(lblServers);
        serverCard.Controls.Add(_txtServerFolder);
        serverCard.Controls.Add(_btnServerBrowse);
        serverCard.Controls.Add(_btnAddServer);
        serverCard.Controls.Add(_lstServers);
        serverCard.Controls.Add(_btnRemoveServer);

        ConfigurePrimaryButton(_btnSave, "Salvar", 560, 498, 105);
        _btnSave.Click += (_, _) => SaveAndClose();

        ConfigureSecondaryButton(_btnCancel, "Cancelar", 675, 498, 105);
        _btnCancel.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(lblTitle);
        Controls.Add(lblSubtitle);
        Controls.Add(sourceCard);
        Controls.Add(serverCard);
        Controls.Add(_btnSave);
        Controls.Add(_btnCancel);
    }

    private static void ConfigurePrimaryButton(Button button, string text, int left, int top, int width)
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
        button.BackColor = Color.FromArgb(14, 116, 144);
        button.ForeColor = Color.White;
    }

    private static void ConfigureSecondaryButton(Button button, string text, int left, int top, int width)
    {
        button.Text = text;
        button.Left = left;
        button.Top = top;
        button.Width = width;
        button.Height = 32;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Cursor = Cursors.Hand;
        button.Font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold);
        button.BackColor = Color.FromArgb(226, 232, 240);
        button.ForeColor = Color.FromArgb(15, 23, 42);
    }

    private void LoadFromSettings()
    {
        _txtSourceFolder.Text = _settings.SourceFolder;
        _lstServers.Items.Clear();
        foreach (var folder in _settings.ServerFolders)
        {
            _lstServers.Items.Add(folder);
        }
    }

    private void SelectSourceFolder()
    {
        using var dialog = new FolderBrowserDialog();
        dialog.Description = "Selecione a pasta de origem dos executaveis";

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _txtSourceFolder.Text = dialog.SelectedPath;
        }
    }

    private void SelectServerFolder()
    {
        using var dialog = new FolderBrowserDialog();
        dialog.Description = "Selecione a pasta de servidor";

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _txtServerFolder.Text = dialog.SelectedPath;
        }
    }

    private void AddServerFolder()
    {
        var path = _txtServerFolder.Text.Trim();
        if (string.IsNullOrWhiteSpace(path))
        {
            MessageBox.Show(this, "Informe uma pasta de servidor.", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_lstServers.Items.Cast<string>().Any(item => string.Equals(item, path, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show(this, "Esse caminho ja foi adicionado.", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _lstServers.Items.Add(path);
        _txtServerFolder.Clear();
        _txtServerFolder.Focus();
    }

    private void RemoveSelectedServer()
    {
        if (_lstServers.SelectedIndex < 0)
        {
            return;
        }

        _lstServers.Items.RemoveAt(_lstServers.SelectedIndex);
    }

    private void SaveAndClose()
    {
        var sourceFolder = _txtSourceFolder.Text.Trim();
        if (string.IsNullOrWhiteSpace(sourceFolder))
        {
            MessageBox.Show(this, "Informe a pasta de origem dos executaveis.", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var servers = _lstServers.Items.Cast<string>().ToList();
        if (servers.Count == 0)
        {
            MessageBox.Show(this, "Adicione ao menos uma pasta de servidor.", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _settings = new AppSettings
        {
            SourceFolder = sourceFolder,
            ServerFolders = servers
        };

        SettingsService.Save(_settings);
        DialogResult = DialogResult.OK;
        Close();
    }
}
