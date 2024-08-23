using System.Reflection;
using System.Windows;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.UserControls.Helps;

namespace MyExpenses.Wpf.UserControls.Helps;

public partial class VersionControl
{
    #region DependencyProperty

    public static readonly DependencyProperty ApplicationVersionProperty =
        DependencyProperty.Register(nameof(ApplicationVersion), typeof(string), typeof(VersionControl),
            new PropertyMetadata(default(string)));

    public string ApplicationVersion
    {
        get => (string)GetValue(ApplicationVersionProperty);
        set => SetValue(ApplicationVersionProperty, value);
    }

    public static readonly DependencyProperty ApplicationVersionValueProperty =
        DependencyProperty.Register(nameof(ApplicationVersionValue), typeof(string), typeof(VersionControl),
            new PropertyMetadata(default(string)));

    public string ApplicationVersionValue
    {
        get => (string)GetValue(ApplicationVersionValueProperty);
        set => SetValue(ApplicationVersionValueProperty, value);
    }

    public static readonly DependencyProperty DatabaseVersionProperty =
        DependencyProperty.Register(nameof(DatabaseVersion), typeof(string), typeof(VersionControl),
            new PropertyMetadata(default(string)));

    public string DatabaseVersion
    {
        get => (string)GetValue(DatabaseVersionProperty);
        set => SetValue(DatabaseVersionProperty, value);
    }

    public static readonly DependencyProperty DatabaseVersionValueProperty =
        DependencyProperty.Register(nameof(DatabaseVersionValue), typeof(string), typeof(VersionControl),
            new PropertyMetadata(default(string)));

    public string DatabaseVersionValue
    {
        get => (string)GetValue(DatabaseVersionValueProperty);
        set => SetValue(DatabaseVersionValueProperty, value);
    }

    #endregion

    public VersionControl()
    {
        UpdateValues();
        UpdateLanguage();

        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    #endregion

    #region Function

    private void UpdateLanguage()
    {
        ApplicationVersion = VersionControlResources.ApplicationVersion;
        DatabaseVersion = VersionControlResources.DatabaseVersion;
    }

    private void UpdateValues()
    {
        SetApplicationVersionValue();
        SetDatabaseVersionValue();
    }

    private void SetDatabaseVersionValue()
    {
        var dbPath = DbContextBackup.LocalFilePathDataBaseModel;
        using var context = new DataBaseContext(dbPath);
        var version = context.TVersions.First();
        
        DatabaseVersionValue = $"{version.Version!.Major}.{version.Version!.Minor}.{version.Version!.Build}";
    }

    private void SetApplicationVersionValue()
    {
        var assembly = Assembly.GetEntryAssembly()!;
        var currentVersion = assembly.GetName().Version!;

        ApplicationVersionValue = $"{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}";
    }

    #endregion
}