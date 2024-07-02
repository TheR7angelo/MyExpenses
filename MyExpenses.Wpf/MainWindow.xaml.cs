using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Dropbox.Api.Files;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.WindowStyle;
using MyExpenses.WebApi.Dropbox;
using MyExpenses.Wpf.Resources.Resx.Windows.MainWindow;
using MyExpenses.Wpf.Utils.FilePicker;
using MyExpenses.Wpf.Windows;
using Serilog;

namespace MyExpenses.Wpf;

public partial class MainWindow
{
    public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.Register(nameof(CanGoBack),
        typeof(bool), typeof(MainWindow), new PropertyMetadata(default(bool)));

    #region MenuItemFile

    public string MenuItemHeaderFile { get; } = MainWindowResources.MenuItemHeaderFile;

    #region MenuItem Database

    public string MenuItemHeaderDatabase { get; } = MainWindowResources.MenuItemHeaderDatabase;

    public string MenuItemHeaderExportDatabase { get; } = MainWindowResources.MenuItemHeaderExportDatabase;
    public string MenuItemHeaderImportDatabase { get; } = MainWindowResources.MenuItemHeaderImportDatabase;

    #endregion

    public string MenuItemHeaderPrevious { get; } = MainWindowResources.MenuItemHeaderPrevious;
    public string MenuItemHeaderSettings { get; } = MainWindowResources.MenuItemHeaderSettings;

    public bool CanGoBack
    {
        get => (bool)GetValue(CanGoBackProperty);
        set => SetValue(CanGoBackProperty, value);
    }

    #endregion

    public MainWindow()
    {
        InitializeComponent();

        var hWnd = new WindowInteropHelper(GetWindow(this)!).EnsureHandle();
        hWnd.SetWindowCornerPreference(DwmWindowCornerPreference.Round);

        Navigator.CanGoBackChanged += Navigator_OnCanGoBackChanged;
    }

    private void Navigator_OnCanGoBackChanged(object? sender, NavigatorEventArgs e)
    {
        CanGoBack = e.CanGoBack;
    }

    private void MenuItemPrevious_OnClick(object sender, RoutedEventArgs e)
        => nameof(FrameBody).GoBack();

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

    private async void MenuItemDatabaseExport_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
        Console.WriteLine("Need export");

        var saveLocationWindow = new SaveLocationWindow();
        saveLocationWindow.ShowDialog();

        if (saveLocationWindow.DialogResult is not true) return;

        var database = DataBaseContext.FilePath!;

        try
        {
            //TODO wait screen
            switch (saveLocationWindow.SaveLocationResult)
            {
                case SaveLocation.Local:
                    await SaveToLocal(database);
                    Log.Information("Database was successfully save to local");
                    break;
                case SaveLocation.Dropbox:
                    await SaveToCloudAsync(database);
                    Log.Information("Database was successfully save to cloud");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occur please retry");
        }

        //TODO make messagebox result
    }

    private static async Task<FileMetadata> SaveToCloudAsync(string database)
    {
        var dropboxService = new DropboxService();
        return await dropboxService.UploadFileAsync(database, DbContextBackup.CloudDirectoryBackupDatabase);
    }

    private static async Task SaveToLocal(string database)
    {
        var sqliteDialog = new SqliteFileDialog();
        var selectedDialog = sqliteDialog.SaveFile();

        if (string.IsNullOrEmpty(selectedDialog)) return;

        selectedDialog = Path.ChangeExtension(selectedDialog, DbContextBackup.Extension);
        await Task.Run(() => File.Copy(database, selectedDialog, true));
    }

    private void MenuItemDatabaseImport_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
        Console.WriteLine("Need import");
    }
}