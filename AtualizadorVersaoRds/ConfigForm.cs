namespace AtualizadorVersaoRds;

public sealed class ConfigForm : Form
{
    private readonly TextBox _txtSourceFolder = new();
    private readonly TextBox _txtServerFolder = new();
    private readonly ListBox _lstServers = new();
    private readonly Button _btnSave = new();
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
        Width = 760;
        Height = 500;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var lblSource = new Label
        {
            Text = "Pasta de origem dos executaveis:",
            Left = 20,
            Top = 20,
            Width = 260
        };

        _txtSourceFolder.Left = 20;
        _txtSourceFolder.Top = 45;
        _txtSourceFolder.Width = 600;

        var btnSourceBrowse = new Button
        {
            Text = "Buscar...",
            Left = 630,
            Top = 43,
            Width = 90
        };
        btnSourceBrowse.Click += (_, _) => SelectSourceFolder();

        var lblServers = new Label
        {
            Text = "Pastas de servidores:",
            Left = 20,
            Top = 90,
            Width = 180
        };

        _txtServerFolder.Left = 20;
        _txtServerFolder.Top = 115;
        _txtServerFolder.Width = 500;

        var btnServerBrowse = new Button
        {
            Text = "Buscar...",
            Left = 530,
            Top = 113,
            Width = 90
        };
        btnServerBrowse.Click += (_, _) => SelectServerFolder();

        var btnAddServer = new Button
        {
            Text = "Adicionar",
            Left = 630,
            Top = 113,
            Width = 90
        };
        btnAddServer.Click += (_, _) => AddServerFolder();

        _lstServers.Left = 20;
        _lstServers.Top = 150;
        _lstServers.Width = 600;
        _lstServers.Height = 220;

        var btnRemoveServer = new Button
        {
            Text = "Remover selecionado",
            Left = 630,
            Top = 150,
            Width = 90,
            Height = 60
        };
        btnRemoveServer.Click += (_, _) => RemoveSelectedServer();

        _btnSave.Text = "Salvar";
        _btnSave.Left = 540;
        _btnSave.Top = 390;
        _btnSave.Width = 90;
        _btnSave.Click += (_, _) => SaveAndClose();

        var btnCancel = new Button
        {
            Text = "Cancelar",
            Left = 630,
            Top = 390,
            Width = 90
        };
        btnCancel.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.Add(lblSource);
        Controls.Add(_txtSourceFolder);
        Controls.Add(btnSourceBrowse);
        Controls.Add(lblServers);
        Controls.Add(_txtServerFolder);
        Controls.Add(btnServerBrowse);
        Controls.Add(btnAddServer);
        Controls.Add(_lstServers);
        Controls.Add(btnRemoveServer);
        Controls.Add(_btnSave);
        Controls.Add(btnCancel);
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
