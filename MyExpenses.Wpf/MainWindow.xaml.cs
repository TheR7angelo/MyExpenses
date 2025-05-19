using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using MyExpenses.Core;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.SharedUtils.Resources.Resx.DashBoardManagement;
using MyExpenses.SharedUtils.Utils;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.Windows.MainWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;

namespace MyExpenses.Wpf;

public partial class MainWindow
{
    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.Register(nameof(CanGoBack),
        typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

    #region MenuItemFile

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty MenuItemHeaderFileProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderFile), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderFile
    {
        get => (string)GetValue(MenuItemHeaderFileProperty);
        set => SetValue(MenuItemHeaderFileProperty, value);
    }

    #region MenuItem Database

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty MenuItemHeaderDatabaseProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderDatabase), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderDatabase
    {
        get => (string)GetValue(MenuItemHeaderDatabaseProperty);
        set => SetValue(MenuItemHeaderDatabaseProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty MenuItemHeaderExportDatabaseProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderExportDatabase), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderExportDatabase
    {
        get => (string)GetValue(MenuItemHeaderExportDatabaseProperty);
        set => SetValue(MenuItemHeaderExportDatabaseProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty MenuItemHeaderImportDatabaseProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderImportDatabase), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderImportDatabase
    {
        get => (string)GetValue(MenuItemHeaderImportDatabaseProperty);
        set => SetValue(MenuItemHeaderImportDatabaseProperty, value);
    }

    #endregion

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty MenuItemHeaderHelpProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderHelp), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderHelp
    {
        get => (string)GetValue(MenuItemHeaderHelpProperty);
        set => SetValue(MenuItemHeaderHelpProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty MenuItemHeaderSettingsProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderSettings), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderSettings
    {
        get => (string)GetValue(MenuItemHeaderSettingsProperty);
        set => SetValue(MenuItemHeaderSettingsProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty MenuItemHeaderPreviousProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderPrevious), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderPrevious
    {
        get => (string)GetValue(MenuItemHeaderPreviousProperty);
        set => SetValue(MenuItemHeaderPreviousProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty MenuItemHeaderVacuumDatabasesProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderVacuumDatabases), typeof(string), typeof(MainWindow),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderVacuumDatabases
    {
        get => (string)GetValue(MenuItemHeaderVacuumDatabasesProperty);
        set => SetValue(MenuItemHeaderVacuumDatabasesProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
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
        // ReSharper disable once HeapView.BoxingAllocation
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

        // ReSharper disable HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
        Navigator.CanGoBackChanged += Navigator_OnCanGoBackChanged;
        // ReSharper restore HeapView.DelegateAllocation
    }

    #region Action

    private void ButtonGithubPage_OnClick(object sender, RoutedEventArgs e)
        => WebUtils.OpenGithubPage();

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
        => _ = DataBaseContext.FilePath!.HandleButtonExportDataBase();

    private void MenuItemHelp_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var helpsWindow = new HelpsWindow();
        helpsWindow.ShowDialog();
    }

    private void MenuItemVacuumDatabases_OnClick(object sender, RoutedEventArgs e)
    {
        var vacuumDatabases = Core.ImportExportUtils.VacuumDatabases();

        if (vacuumDatabases.Contains(null))
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

        var notNullVacuumDatabases = vacuumDatabases.Where(s => s is not null);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var vacuumDatabaseUpdateWindow = new VacuumDatabaseUpdateWindow(notNullVacuumDatabases!);
        vacuumDatabaseUpdateWindow.ShowDialog();
    }

    private void MenuItemVacuumDatabase_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var existingDatabase = new ExistingDatabase(DataBaseContext.FilePath!);
        var sizeDatabase = existingDatabase.VacuumDatabase();
        if (sizeDatabase is not null)
        {
            MsgBox.Show(MainWindowResources.MessageBoxMenuItemVacuumDatabaseSucess, MsgBoxImage.Check,
                MessageBoxButton.OK);

            // ReSharper disable once HeapView.ObjectAllocation.Evident
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
        // ReSharper disable once HeapView.ObjectAllocation.Evident
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
        MenuItemHeaderExportDatabase = DashBoardManagementResources.MenuItemHeaderExportDatabase;
        MenuItemHeaderImportDatabase = MainWindowResources.MenuItemHeaderImportDatabase;
        MenuItemHeaderVacuumDatabases = MainWindowResources.MenuItemHeaderVacuumDatabases;
        MenuItemHeaderVacuumDatabase = MainWindowResources.MenuItemHeaderVacuumDatabase;

        MenuItemHeaderHelp = MainWindowResources.MenuItemHeaderHelp;

        MenuItemHeaderSettings = MainWindowResources.MenuItemHeaderSettings;
        MenuItemHeaderPrevious = MainWindowResources.MenuItemHeaderPrevious;
    }

    #endregion
}