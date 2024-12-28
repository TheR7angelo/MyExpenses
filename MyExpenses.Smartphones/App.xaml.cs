using System.Globalization;
using System.Reflection;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO.Smartphones;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using Serilog;

namespace MyExpenses.Smartphones;

public partial class App
{
    public static CancellationTokenSource CancellationTokenSource { get; private set; } = null!;

    public App()
    {
        SetInitialFile();

        CancellationTokenSource = new CancellationTokenSource();

        Log.Logger = LoggerConfig.CreateConfig();
        Log.Information("Starting the application");

        Log.Information("Reading configuration file");
        var configuration = Config.Configuration;
        Log.Information("Configuration read :{NewLine}{Configuration}", Environment.NewLine, configuration);

        Log.Information("Apply log configuration");
        LoadLogConfiguration(configuration.System.MaxDaysLog);

        Log.Information("Start of database backup on start");
        var totalDatabaseBackup = DbContextBackup.BackupDatabase();
        var totalDatabaseDelete = DbContextBackup.CleanBackupDatabase(configuration.System.MaxBackupDatabase);
        Log.Information("{TotalDatabaseDelete} backup(s) database has been deleted", totalDatabaseDelete);
        Log.Information("{TotalDatabaseBackup} database(s) has been backed up", totalDatabaseBackup);

        AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

        Log.Information("Apply interface configuration");
        LoadInterfaceConfiguration(configuration.Interface);

        InitializeComponent();

        MainPage = new AppShell();
    }

    private static void LoadInterfaceConfiguration(Interface configurationInterface)
    {
        // LoadInterfaceTheme(configurationInterface.Theme);
        LoadInterfaceLanguage(configurationInterface.Language);
    }

    private static void LoadInterfaceLanguage(string? cultureInfoCode)
    {
        var currentCultureIsSupported = false;

        if (string.IsNullOrEmpty(cultureInfoCode))
        {
            var currentCurrentCulture = CultureInfo.CurrentUICulture.Name;

            using var context = new DataBaseContext(DbContextBackup.LocalFilePathDataBaseModel);

            currentCultureIsSupported = context.TSupportedLanguages.Any(s => s.Code == currentCurrentCulture);
            cultureInfoCode = currentCultureIsSupported
                ? currentCurrentCulture
                : context.TSupportedLanguages.First(s => (bool)s.DefaultLanguage!).Code;

            var configuration = Config.Configuration;
            configuration.Interface.Language = cultureInfoCode;
            configuration.WriteConfiguration();
        }

        var cultureInfo = new CultureInfo(cultureInfoCode);
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        if (currentCultureIsSupported) DbContextHelper.UpdateDbLanguage();
    }

    // public static void LoadInterfaceTheme(Theme configurationTheme)
    // {
    //     var baseTheme = (BaseTheme)configurationTheme.BaseTheme;
    //     var primaryColor = configurationTheme.HexadecimalCodePrimaryColor.ToColor() ?? Color.FromRgb(0, 128, 0);
    //     var secondaryColor = configurationTheme.HexadecimalCodeSecondaryColor.ToColor() ?? Color.FromRgb(255, 165, 0);
    //
    //     var paletteHelper = new PaletteHelper();
    //     var theme = paletteHelper.GetTheme();
    //     theme.SetBaseTheme(baseTheme);
    //     theme.SetPrimaryColor(primaryColor);
    //     theme.SetSecondaryColor(secondaryColor);
    //
    //     paletteHelper.SetTheme(theme);
    // }

    private static void LoadLogConfiguration(int logMaxDays)
    {
        var numberOfLogDeleted = LoggerConfig.RemoveOldLog(logMaxDays);
        Log.Information("{NumberOfLogDeleted} log was deleted", numberOfLogDeleted);
    }

    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex) Log.Fatal(ex, "An unexpected error occurred");
        else Log.Fatal("An unexpected error with no associated exception occurred");

        Log.CloseAndFlush();
    }

    private static void CurrentDomainOnProcessExit(object? sender, EventArgs e)
    {
        Log.Information("Start of database backup on exit");
        DbContextBackup.BackupDatabase();

        Log.Information("Application exit");
        Log.CloseAndFlush();
    }

    private void SetInitialFile()
    {
        var jsonFile = Path.Join(DbContextBackup.OsBasePath, "AppVersionInfo.json");

        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        var needUpdateFiles = false;
        if (File.Exists(jsonFile))
        {
            var appVersionInfo = jsonFile.ToObject<AppVersionInfo>()!;
            if (currentVersion > appVersionInfo.Version)
            {
                needUpdateFiles = true;
            }
        }
        else
        {
            var appVersionInfo = new AppVersionInfo
            {
                Version = currentVersion,
                LastUpdated = DateTime.Now
            };
            var json = appVersionInfo.ToJson();
            File.WriteAllText(jsonFile, json);

            needUpdateFiles = true;
        }

        if (!needUpdateFiles) return;

        FileManager.AddAllFiles();
    }
}