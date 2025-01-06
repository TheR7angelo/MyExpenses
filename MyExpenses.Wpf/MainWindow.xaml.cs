using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using MyExpenses.Core.Export;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.WebApi.Dropbox;
using MyExpenses.Wpf.Resources.Resx.Windows.MainWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Utils.FilePicker;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using MyExpenses.Wpf.Windows.SaveLocationWindow;
using Serilog;

namespace MyExpenses.Wpf;

public partial class MainWindow
{
    public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.Register(nameof(CanGoBack),
        typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

    #region MenuItemFile

    public static readonly DependencyProperty MenuItemHeaderFileProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderFile), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderFile
    {
        get => (string)GetValue(MenuItemHeaderFileProperty);
        set => SetValue(MenuItemHeaderFileProperty, value);
    }

    #region MenuItem Database

    public static readonly DependencyProperty MenuItemHeaderDatabaseProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderDatabase), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderDatabase
    {
        get => (string)GetValue(MenuItemHeaderDatabaseProperty);
        set => SetValue(MenuItemHeaderDatabaseProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderExportDatabaseProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderExportDatabase), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderExportDatabase
    {
        get => (string)GetValue(MenuItemHeaderExportDatabaseProperty);
        set => SetValue(MenuItemHeaderExportDatabaseProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderImportDatabaseProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderImportDatabase), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderImportDatabase
    {
        get => (string)GetValue(MenuItemHeaderImportDatabaseProperty);
        set => SetValue(MenuItemHeaderImportDatabaseProperty, value);
    }

    #endregion

    public static readonly DependencyProperty MenuItemHeaderHelpProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderHelp), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderHelp
    {
        get => (string)GetValue(MenuItemHeaderHelpProperty);
        set => SetValue(MenuItemHeaderHelpProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderSettingsProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderSettings), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderSettings
    {
        get => (string)GetValue(MenuItemHeaderSettingsProperty);
        set => SetValue(MenuItemHeaderSettingsProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderPreviousProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderPrevious), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderPrevious
    {
        get => (string)GetValue(MenuItemHeaderPreviousProperty);
        set => SetValue(MenuItemHeaderPreviousProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderVacuumDatabasesProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderVacuumDatabases), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderVacuumDatabases
    {
        get => (string)GetValue(MenuItemHeaderVacuumDatabasesProperty);
        set => SetValue(MenuItemHeaderVacuumDatabasesProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderVacuumDatabaseProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderVacuumDatabase), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderVacuumDatabase
    {
        get => (string)GetValue(MenuItemHeaderVacuumDatabaseProperty);
        set => SetValue(MenuItemHeaderVacuumDatabaseProperty, value);
    }

    public string ApplicationName { get; }

    public bool CanGoBack
    {
        get => (bool)GetValue(CanGoBackProperty);
        set => SetValue(CanGoBackProperty, value);
    }

    #endregion

    public delegate void VaccumDatabaseEventHandler();

    public static event VaccumDatabaseEventHandler? VaccumDatabase;

    public MainWindow()
    {
        var assembly = Assembly.GetEntryAssembly()!;
        ApplicationName = assembly.GetName().Name!;

        UpdateLanguage();

        InitializeComponent();

        this.SetWindowCornerPreference();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
        Navigator.CanGoBackChanged += Navigator_OnCanGoBackChanged;
    }

    #region Action

    private void ButtonGithubPage_OnClick(object sender, RoutedEventArgs e)
        => MyExpenses.Utils.Utils.OpenGithubPage();

    private void FrameBody_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.XButton1)
        {
            e.Handled = true;
            nameof(FrameBody).GoBack();
        }
        else if (e.ChangedButton == MouseButton.XButton2)
        {
            e.Handled = true;
            nameof(FrameBody).GoForward();
        }
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
        => App.CancellationTokenSource.Cancel();

    private void MenuItemDatabaseExport_OnClick(object sender, RoutedEventArgs e)
        => _ = HandleMenuItemDatabaseExport();

    private void MenuItemHelp_OnClick(object sender, RoutedEventArgs e)
    {
        var helpsWindow = new HelpsWindow();
        helpsWindow.ShowDialog();
    }

    private void MenuItemVacuumDatabases_OnClick(object sender, RoutedEventArgs e)
    {
        var listSuccess = new List<bool>();
        var sizeDatabases = new List<SizeDatabase>();
        foreach (var existingDatabase in DbContextBackup.GetExistingDatabase())
        {
            var oldSize = existingDatabase.FileInfo.Length;
            var result = VacuumDatabase(existingDatabase.FilePath);
            listSuccess.Add(result);

            if (result is not true) continue;

            var newSize = new ExistingDatabase(existingDatabase.FilePath).FileInfo.Length;
            var sizeDatabase = new SizeDatabase { FileNameWithoutExtension = existingDatabase.FileNameWithoutExtension };
            sizeDatabase.SetOldSize(oldSize);
            sizeDatabase.SetNewSize(newSize);

            sizeDatabases.Add(sizeDatabase);
        }

        if (listSuccess.Contains(false))
        {
            MsgBox.Show(MainWindowResources.MessageBoxMenuItemVacuumDatabasesError, MsgBoxImage.Error,
                MessageBoxButton.OK);
        }
        else
        {
            MsgBox.Show(MainWindowResources.MessageBoxMenuItemVacuumDatabasesSucess, MsgBoxImage.Check,
                MessageBoxButton.OK);
        }

        VaccumDatabase?.Invoke();

        if (!listSuccess.Any(s => s)) return;

        var vacuumDatabaseUpdateWindow = new VacuumDatabaseUpdateWindow(sizeDatabases);
        vacuumDatabaseUpdateWindow.ShowDialog();
    }

    private void MenuItemVacuumDatabase_OnClick(object sender, RoutedEventArgs e)
    {
        var existingDatabase = new ExistingDatabase(DataBaseContext.FilePath!);

        var oldSize = existingDatabase.FileInfo.Length;
        var result = VacuumDatabase();
        if (result)
        {
            MsgBox.Show(MainWindowResources.MessageBoxMenuItemVacuumDatabaseSucess, MsgBoxImage.Check,
                MessageBoxButton.OK);

            var newSize = new ExistingDatabase(DataBaseContext.FilePath!).FileInfo.Length;
            var sizeDatabase = new SizeDatabase { FileNameWithoutExtension = existingDatabase.FileNameWithoutExtension };
            sizeDatabase.SetOldSize(oldSize);
            sizeDatabase.SetNewSize(newSize);

            var vacuumDatabaseUpdateWindow = new VacuumDatabaseUpdateWindow(sizeDatabase);
            vacuumDatabaseUpdateWindow.ShowDialog();
        }
        else
        {
            MsgBox.Show(MainWindowResources.MessageBoxMenuItemVacuumDatabaseError, MsgBoxImage.Error,
                MessageBoxButton.OK);
        }

        VaccumDatabase?.Invoke();
    }

    private void MenuItemSetting_OnClick(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow();
        settingsWindow.ShowDialog();
    }

    private void MenuItemPrevious_OnClick(object sender, RoutedEventArgs e)
        => nameof(FrameBody).GoBack();

    private void Navigator_OnCanGoBackChanged(object? sender, NavigatorEventArgs e)
        => CanGoBack = e.CanGoBack;

    #endregion

    #region Function

    private static async Task HandleMenuItemDatabaseExport()
    {
        var saveLocation = SaveLocationUtils.GetExportSaveLocation();
        if (saveLocation is null) return;

        var database = DataBaseContext.FilePath!;

        var waitScreenWindow = new WaitScreenWindow();
        try
        {
            switch (saveLocation)
            {
                case SaveLocation.Database:
                    waitScreenWindow.WaitMessage = MainWindowResources.MenuItemDatabaseExportWaitMessageExportToLocal;
                    waitScreenWindow.Show();
                    await SaveToLocal(database);
                    break;

                case SaveLocation.Folder:
                    waitScreenWindow.WaitMessage = MainWindowResources.MenuItemDatabaseExportWaitMessageExportToLocal;
                    waitScreenWindow.Show();
                    await ExportToLocalFolderAsync(database, false);
                    break;

                case SaveLocation.Compress:
                    waitScreenWindow.WaitMessage = MainWindowResources.MenuItemDatabaseExportWaitMessageExportToLocal;
                    waitScreenWindow.Show();
                    await ExportToLocalFolderAsync(database, true);
                    break;

                case SaveLocation.Dropbox:
                    waitScreenWindow.WaitMessage = MainWindowResources.MenuItemDatabaseExportWaitMessageExportToCloud;
                    waitScreenWindow.Show();
                    await SaveToCloudAsync(database);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            waitScreenWindow.Close();
            MsgBox.Show(MainWindowResources.MenuItemDatabaseExportSucess, MsgBoxImage.Check);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            waitScreenWindow.Close();

            MsgBox.Show(MainWindowResources.MenuItemDatabaseExportError, MsgBoxImage.Warning);
        }
    }

    private void UpdateLanguage()
    {
        MenuItemHeaderFile = MainWindowResources.MenuItemHeaderFile;

        MenuItemHeaderDatabase = MainWindowResources.MenuItemHeaderDatabase;
        MenuItemHeaderExportDatabase = MainWindowResources.MenuItemHeaderExportDatabase;
        MenuItemHeaderImportDatabase = MainWindowResources.MenuItemHeaderImportDatabase;
        MenuItemHeaderVacuumDatabases = MainWindowResources.MenuItemHeaderVacuumDatabases;
        MenuItemHeaderVacuumDatabase = MainWindowResources.MenuItemHeaderVacuumDatabase;

        MenuItemHeaderHelp = MainWindowResources.MenuItemHeaderHelp;

        MenuItemHeaderSettings = MainWindowResources.MenuItemHeaderSettings;
        MenuItemHeaderPrevious = MainWindowResources.MenuItemHeaderPrevious;
    }

    // private static async Task<FileMetadata> SaveToCloudAsync(string database)
    private static async Task SaveToCloudAsync(string database)
    {
        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Wpf);
        Log.Information("Starting to upload {FileName} to cloud storage", Path.GetFileName(database));
        _ = await dropboxService.UploadFileAsync(database, DbContextBackup.CloudDirectoryBackupDatabase);
        Log.Information("Successfully uploaded {FileName} to cloud storage", Path.GetFileName(database));

        // return fileMetadata;
    }

    private static async Task ExportToLocalFolderAsync(string databaseFilePath, bool isCompress)
    {
        var folderDialog = new FolderDialog();
        var selectedDialog = folderDialog.GetFile();

        if (string.IsNullOrEmpty(selectedDialog))
        {
            Log.Warning("Export cancelled. No file path provided");
            return;
        }

        Log.Information("Starting to export database to {SelectedDialog}", selectedDialog);

        var success = false;
        await Task.Run(async () =>
        {
            var existingDatabase = new ExistingDatabase(databaseFilePath);
            success = await existingDatabase.ToFolderAsync(selectedDialog, isCompress);
        });

        if (!success)
        {
            Log.Error("An error occured while exporting the database");
            MsgBox.Show(MainWindowResources.MessageBoxErrorExportToLocalFolder, MsgBoxImage.Error, MessageBoxButton.OK);
            return;
        }
        Log.Information("Database successfully copied to local storage");

        var response = MsgBox.Show(MainWindowResources.MessageBoxOpenExportFolderQuestion, MsgBoxImage.Question,
            MessageBoxButton.YesNo);
        if (response is MessageBoxResult.Yes) selectedDialog.StartFile();
    }

    private static async Task SaveToLocal(string database)
    {
        var sqliteDialog = new SqliteFileDialog(Path.GetFileName(database));
        var selectedDialog = sqliteDialog.SaveFile();

        if (string.IsNullOrEmpty(selectedDialog))
        {
            Log.Warning("Export cancelled. No file path provided");
            return;
        }

        selectedDialog = Path.ChangeExtension(selectedDialog, DbContextBackup.Extension);
        Log.Information("Starting to copy database to {SelectedDialog}", selectedDialog);
        await Task.Run(() => { File.Copy(database, selectedDialog, true); });
        Log.Information("Database successfully copied to local storage");

        var parentDirectory = Path.GetDirectoryName(selectedDialog)!;
        var response = MsgBox.Show(MainWindowResources.MessageBoxOpenExportFolderQuestion, MsgBoxImage.Question,
            MessageBoxButton.YesNo);
        if (response is MessageBoxResult.Yes) parentDirectory.StartFile();
    }

    private static bool VacuumDatabase(string? dataBaseFilePath = null)
    {
        dataBaseFilePath ??= DataBaseContext.FilePath;

        Log.Information("Starting to vacuum database: {DatabasePath}", dataBaseFilePath);

        try
        {
            var row = "VACUUM ;".ExecuteRawSql(dataBaseFilePath);
            Log.Information("Database vacuumed successfully");
            Log.Information("Number of rows affected: {Row}", row);
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e, "An error occured while vacuuming the database");
            return false;
        }
    }

    #endregion
}