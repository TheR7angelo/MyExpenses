using System.Globalization;
using System.Reflection;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.IO.Smartphones;
using MyExpenses.SharedUtils.GlobalInfos;
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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // A new instance of CancellationTokenSource is created to manage cancellation tokens across various
        // tasks in the application. This allows proper handling and cooperative cancellation of asynchronous
        // operations, ensuring resources are released effectively when tasks are canceled.
        CancellationTokenSource = new CancellationTokenSource();

        Log.Logger = LoggerConfig.CreateConfig(null);
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
    }

    // A new instance of Window is created with an AppShell object as its root.
    // This ensures that the application's main UI structure is properly initialized,
    // providing the entry point for navigation and user interaction within the app.
    protected override Window CreateWindow(IActivationState? activationState)
        // ReSharper disable HeapView.ObjectAllocation.Evident
        => new(new AppShell());
        // ReSharper restore HeapView.ObjectAllocation.Evident


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

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // A new instance of DataBaseContext is created using the specified database file path.
            // The "using" statement ensures that the database context is properly disposed of
            // after use, releasing any resources it holds and maintaining efficient resource management.
            using var context = new DataBaseContext(DatabaseInfos.LocalFilePathDataBaseModel);

            currentCultureIsSupported = context.TSupportedLanguages.Any(s => s.Code == currentCurrentCulture);
            cultureInfoCode = currentCultureIsSupported
                ? currentCurrentCulture
                : context.TSupportedLanguages.First(s => (bool)s.DefaultLanguage!).Code;

            var configuration = Config.Configuration;
            configuration.Interface.Language = cultureInfoCode;
            configuration.WriteConfiguration();
        }

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // A new CultureInfo object is created using the specified cultureInfoCode.
        // This object represents information about a specific culture (such as language and regional settings)
        // and is used to configure cultural aspects of the application, like formatting and localization.
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
        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        var needUpdateFiles = false;
        if (File.Exists(OsInfos.AppVersionInfo))
        {
            var appVersionInfo = OsInfos.AppVersionInfo.ToObject<AppVersionInfo>()!;
            if (currentVersion > appVersionInfo.Version)
            {
                needUpdateFiles = true;
            }
        }
        else
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // A new instance of AppVersionInfo is created and initialized with the current version of the application
            // and the current timestamp. This provides metadata about the application's version and the last update
            // time, which can later be used for version control or update tracking purposes.
            var appVersionInfo = new AppVersionInfo
            {
                Version = currentVersion,
                LastUpdated = DateTime.Now
            };
            var json = appVersionInfo.ToJson();
            File.WriteAllText(OsInfos.AppVersionInfo, json);

            needUpdateFiles = true;
        }

        if (!needUpdateFiles) return;

        FileManager.AddAllFiles();
    }
}