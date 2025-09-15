using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FilterDataGrid;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.SharedUtils.Resources.Resx.BackupSelectorRestoreManagement;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Utils.FilterDataGrid;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Windows;

public partial class BackupSelectorRestoreWindow
{
    public static readonly DependencyProperty ButtonContentRestoreProperty =
        DependencyProperty.Register(nameof(ButtonContentRestore), typeof(string), typeof(BackupSelectorRestoreWindow),
            new PropertyMetadata(default(string)));

    public string ButtonContentRestore
    {
        get => (string)GetValue(ButtonContentRestoreProperty);
        set => SetValue(ButtonContentRestoreProperty, value);
    }

    public static readonly DependencyProperty DateFormatStringProperty =
        DependencyProperty.Register(nameof(DateFormatString), typeof(string), typeof(BackupSelectorRestoreWindow),
            new PropertyMetadata(default(string)));

    public string DateFormatString
    {
        get => (string)GetValue(DateFormatStringProperty);
        init => SetValue(DateFormatStringProperty, value);
    }

    public static readonly DependencyProperty LocalLanguageProperty = DependencyProperty.Register(nameof(LocalLanguage),
        typeof(Local), typeof(BackupSelectorRestoreWindow), new PropertyMetadata(default(Local)));

    public Local LocalLanguage
    {
        get => (Local)GetValue(LocalLanguageProperty);
        init => SetValue(LocalLanguageProperty, value);
    }

    public static readonly DependencyProperty WindowTitleProperty = DependencyProperty.Register(nameof(WindowTitle),
        typeof(string), typeof(BackupSelectorRestoreWindow), new PropertyMetadata(default(string)));

    public string WindowTitle
    {
        get => (string)GetValue(WindowTitleProperty);
        set => SetValue(WindowTitleProperty, value);
    }

    public static readonly DependencyProperty SelectedExistingDatabaseProperty = DependencyProperty.Register(nameof(SelectedExistingDatabase),
        typeof(ExistingDatabase), typeof(BackupSelectorRestoreWindow), new PropertyMetadata(default(ExistingDatabase)));

    public ExistingDatabase SelectedExistingDatabase
    {
        get => (ExistingDatabase)GetValue(SelectedExistingDatabaseProperty);
        set => SetValue(SelectedExistingDatabaseProperty, value);
    }

    public List<ExistingDatabase> ExistingDatabases { get; } = [];
    public ObservableCollection<AExistingDatabase> ExistingDatabasesBackup { get; } = [];

    public BackupSelectorRestoreWindow(IEnumerable<ExistingDatabase> existingDatabases)
    {
        ExistingDatabases.AddRange(existingDatabases.OrderBy(s => s.FileNameWithoutExtension));

        LocalLanguage = CultureInfo.CurrentUICulture.ToLocal();
        DateFormatString = SharedUtils.Converters.DateTimeToDateTimeWithoutSecondsConverter.GetDateTimePattern();

        InitializeComponent();

        UpdaterLanguage();

        this.SetWindowCornerPreference();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged()
        => UpdaterLanguage();

    private void UpdaterLanguage()
    {
        WindowTitle = BackupSelectorRestoreManagementResources.WindowTitle;

        TextColumnDatabaseName.Header = BackupSelectorRestoreManagementResources.TextColumnDatabaseNameHeader;
        TextColumnDatabaseDateBackup.Header = BackupSelectorRestoreManagementResources.TextColumnDatabaseDateBackupHeader;
        ButtonContentRestore = BackupSelectorRestoreManagementResources.ButtonContentRestore;
    }

    private void ButtonDatabase_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        if (button.DataContext is not ExistingDatabase existingDatabase) return;

        SelectedExistingDatabase = existingDatabase;

        var localDirectoryBackupDatabase = Path.Join(DatabaseInfos.LocalDirectoryBackupDatabase, existingDatabase.FileNameWithoutExtension);
        var databases = Directory.GetFiles(localDirectoryBackupDatabase, DatabaseInfos.SearchPatternExtension);

        ExistingDatabasesBackup.Clear();
        foreach (var database in databases)
        {
            var existingDatabaseBackup = new AExistingDatabase(database);
            ExistingDatabasesBackup.Add(existingDatabaseBackup);
        }
    }

    private void ButtonRestoreBackup_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.MsgBox.Show(BackupSelectorRestoreManagementResources.MessageboxRestoreDatabaseQuestionTitle,
            BackupSelectorRestoreManagementResources.MessageboxRestoreDatabaseQuestionMessage,
            MessageBoxButton.YesNo, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) return;

        var existingDatabaseSelected = ExistingDatabasesBackup.FirstOrDefault(s => s.IsSelected);
        if (existingDatabaseSelected is null)
        {
            MsgBox.MsgBox.Show(BackupSelectorRestoreManagementResources.MessageboxNoDatabaseSelectedTitle,
                BackupSelectorRestoreManagementResources.MessageboxNoDatabaseSelectedMessage, MessageBoxButton.OK, MsgBoxImage.Error);
            return;
        }

        Log.Information("Starting to restore the database {FileNameWithoutExtension} with the backup {FileNameWithoutExtensionBackup}", SelectedExistingDatabase.FileNameWithoutExtension, existingDatabaseSelected.FileNameWithoutExtension);
        try
        {
            File.Copy(existingDatabaseSelected.FilePath, SelectedExistingDatabase.FilePath, true);

            Log.Information("Successfully restored the database {FileNameWithoutExtension} with the backup {FileNameWithoutExtensionBackup}", SelectedExistingDatabase.FileNameWithoutExtension, existingDatabaseSelected.FileNameWithoutExtension);
            MsgBox.MsgBox.Show(BackupSelectorRestoreManagementResources.MessageboxRestoreDatabaseSuccessTitle,
                BackupSelectorRestoreManagementResources.MessageboxRestoreDatabaseSuccessMessage, MessageBoxButton.OK, MsgBoxImage.Check);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");

            MsgBox.MsgBox.Show(BackupSelectorRestoreManagementResources.MessageboxRestoreDatabaseErrorTitle,
                BackupSelectorRestoreManagementResources.MessageboxRestoreDatabaseErrorMessage, MessageBoxButton.OK, MsgBoxImage.Error);
        }
    }
}