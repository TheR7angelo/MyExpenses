﻿using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Dropbox.Api.Files;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.Sql.Context;
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
        typeof(bool), typeof(MainWindow), new PropertyMetadata(default(bool)));

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

    public MainWindow()
    {
        var assembly = Assembly.GetEntryAssembly()!;
        ApplicationName = assembly.GetName().Name!;

        Interface.LanguageChanged += Interface_OnLanguageChanged;
        UpdateLanguage();

        InitializeComponent();

        this.SetWindowCornerPreference();

        Navigator.CanGoBackChanged += Navigator_OnCanGoBackChanged;
    }

    #region Action

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

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private async void MenuItemDatabaseExport_OnClick(object sender, RoutedEventArgs e)
    {
        var saveLocation = SaveLocationUtils.GetExportSaveLocation();
        if (saveLocation is null) return;

        var database = DataBaseContext.FilePath!;

        var waitScreenWindow = new WaitScreenWindow();
        try
        {
            switch (saveLocation)
            {
                case SaveLocation.Local:
                    waitScreenWindow.WaitMessage = MainWindowResources.MenuItemDatabaseExportWaitMessageExportToLocal;
                    waitScreenWindow.Show();
                    await SaveToLocal(database);
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

    private void MenuItemDatabaseImport_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
        Console.WriteLine("Need import");
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
            var sizeDatabase = new SizeDatabase
            {
                FileNameWithoutExtension = existingDatabase.FileNameWithoutExtension,
                OldSize = oldSize,
                NewSize = newSize
            };
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

        if (!listSuccess.Any(s => s)) return;

        // TODO work
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
            var sizeDatabase = new SizeDatabase
            {
                FileNameWithoutExtension = existingDatabase.FileNameWithoutExtension,
                OldSize = oldSize,
                NewSize = newSize
            };

            // TODO work
            var vacuumDatabaseUpdateWindow = new VacuumDatabaseUpdateWindow(sizeDatabase);
            vacuumDatabaseUpdateWindow.ShowDialog();
        }
        else
        {
            MsgBox.Show(MainWindowResources.MessageBoxMenuItemVacuumDatabaseError, MsgBoxImage.Error,
                MessageBoxButton.OK);
        }
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

    private void UpdateLanguage()
    {
        MenuItemHeaderFile = MainWindowResources.MenuItemHeaderFile;

        MenuItemHeaderDatabase = MainWindowResources.MenuItemHeaderDatabase;
        MenuItemHeaderExportDatabase = MainWindowResources.MenuItemHeaderExportDatabase;
        MenuItemHeaderImportDatabase = MainWindowResources.MenuItemHeaderImportDatabase;
        MenuItemHeaderVacuumDatabases = MainWindowResources.MenuItemHeaderVacuumDatabases;
        MenuItemHeaderVacuumDatabase = MainWindowResources.MenuItemHeaderVacuumDatabase;

        MenuItemHeaderSettings = MainWindowResources.MenuItemHeaderSettings;
        MenuItemHeaderPrevious = MainWindowResources.MenuItemHeaderPrevious;
    }

    private static async Task<FileMetadata> SaveToCloudAsync(string database)
    {
        var dropboxService = new DropboxService();
        Log.Information("Starting to upload {FileName} to cloud storage", Path.GetFileName(database));
        var fileMetadata = await dropboxService.UploadFileAsync(database, DbContextBackup.CloudDirectoryBackupDatabase);
        Log.Information("Successfully uploaded {FileName} to cloud storage", Path.GetFileName(database));

        return fileMetadata;
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
    }

    private static bool VacuumDatabase(string? dataBaseFilePath = null)
    {
        dataBaseFilePath ??= DataBaseContext.FilePath;

        Log.Information("Starting to vacuum database: {DatabasePath}", dataBaseFilePath);

        try
        {
            "VACUUM ;".ExecuteRawSql(dataBaseFilePath);
            Log.Information("Database vacuumed successfully");
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