using System.Reflection;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.UserControls.Helps.VersionControl;

namespace MyExpenses.Wpf.UserControls.Helps;

public partial class VersionControl
{
    #region DependencyProperty

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ApplicationVersionProperty =
        DependencyProperty.Register(nameof(ApplicationVersion), typeof(string), typeof(VersionControl),
            new PropertyMetadata(default(string)));

    public string ApplicationVersion
    {
        get => (string)GetValue(ApplicationVersionProperty);
        set => SetValue(ApplicationVersionProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ApplicationVersionValueProperty =
        DependencyProperty.Register(nameof(ApplicationVersionValue), typeof(string), typeof(VersionControl),
            new PropertyMetadata(default(string)));

    public string ApplicationVersionValue
    {
        get => (string)GetValue(ApplicationVersionValueProperty);
        set => SetValue(ApplicationVersionValueProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty DatabaseVersionProperty =
        DependencyProperty.Register(nameof(DatabaseVersion), typeof(string), typeof(VersionControl),
            new PropertyMetadata(default(string)));

    public string DatabaseVersion
    {
        get => (string)GetValue(DatabaseVersionProperty);
        set => SetValue(DatabaseVersionProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty DatabaseVersionValueProperty =
        DependencyProperty.Register(nameof(DatabaseVersionValue), typeof(string), typeof(VersionControl),
            new PropertyMetadata(default(string)));

    public string DatabaseVersionValue
    {
        get => (string)GetValue(DatabaseVersionValueProperty);
        set => SetValue(DatabaseVersionValueProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty SqliteVersionProperty = DependencyProperty.Register(nameof(SqliteVersion),
        typeof(string), typeof(VersionControl), new PropertyMetadata(default(string)));

    public string SqliteVersion
    {
        get => (string)GetValue(SqliteVersionProperty);
        set => SetValue(SqliteVersionProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty SqliteVersionValueProperty =
        DependencyProperty.Register(nameof(SqliteVersionValue), typeof(string), typeof(VersionControl),
            new PropertyMetadata(default(string)));

    public string SqliteVersionValue
    {
        get => (string)GetValue(SqliteVersionValueProperty);
        set => SetValue(SqliteVersionValueProperty, value);
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

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion

    #region Function

    private void UpdateLanguage()
    {
        ApplicationVersion = VersionControlResources.ApplicationVersion;
        DatabaseVersion = VersionControlResources.DatabaseVersion;
        SqliteVersion = VersionControlResources.SqliteVersion;
    }

    private void UpdateValues()
    {
        SetApplicationVersionValue();
        SetDatabaseVersionValue();
        SetSqliteVersionValue();
    }

    private void SetSqliteVersionValue()
    {
        var dbPath = DatabaseInfos.LocalFilePathDataBaseModel;
        using var context = new DataBaseContext(dbPath);

        var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT sqlite_version();";

        context.Database.OpenConnection();

        using var reader = command.ExecuteReader();
        reader.Read();
        SqliteVersionValue = reader.GetString(0);

        context.Database.CloseConnection();
    }

    private void SetDatabaseVersionValue()
    {
        using var context = new DataBaseContext(DatabaseInfos.LocalFilePathDataBaseModel);
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