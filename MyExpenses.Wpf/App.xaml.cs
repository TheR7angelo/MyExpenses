using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Systems;
using MyExpenses.Wpf.Utils;
using Log = Serilog.Log;
using Theme = MyExpenses.Models.Config.Interfaces.Theme;

namespace MyExpenses.Wpf;


public partial class App
{
    public static CancellationTokenSource CancellationTokenSource { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

        var systemArgs = e.Args.GetArguments();
        DataBaseContext.LogEventLevel = systemArgs.LogEventLevel;
        DataBaseContext.LogEfCore = systemArgs.LogEfCore;
        DataBaseContext.WriteToFileEfCore = systemArgs.WriteToFileEfCore;
        Log.Logger = LoggerConfig.CreateConfig(systemArgs.LogEventLevel);
        Log.Information("Logger created with log event level: {SystemArgsLogEventLevel}", systemArgs.LogEventLevel);
        Log.Information("Starting the application");

        try
        {
            base.OnStartup(e);

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            CancellationTokenSource = new CancellationTokenSource();

            var iconPath = Path.Join("Resources", "Assets", "Applications", "Icon Resize.png");
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var splashScreenWindow = new SplashScreen(iconPath);
            splashScreenWindow.Show(true, true);

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

            Log.Information("Apply interface configuration");
            LoadInterfaceConfiguration(configuration.Interface);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    private static void LoadInterfaceConfiguration(Interface configurationInterface)
    {
        LoadInterfaceTheme(configurationInterface.Theme);
        LoadInterfaceLanguage(configurationInterface.Language);
    }

    public static void LoadInterfaceLanguage(string? cultureInfoCode)
    {
        var currentCultureIsSupported = false;

        if (string.IsNullOrEmpty(cultureInfoCode))
        {
            // ReSharper disable once HeapView.ClosureAllocation
            var currentCurrentCulture = CultureInfo.CurrentUICulture.Name;

            // ReSharper disable once HeapView.ObjectAllocation.Evident
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
        var cultureInfo = new CultureInfo(cultureInfoCode);
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        if (currentCultureIsSupported) DbContextHelper.UpdateDbLanguage();
    }

    public static void LoadInterfaceTheme(Theme configurationTheme)
    {
        var baseTheme = (BaseTheme)configurationTheme.BaseTheme;
        var primaryColor = configurationTheme.HexadecimalCodePrimaryColor.ToColor() ?? Color.FromRgb(0, 128, 0);
        var secondaryColor = configurationTheme.HexadecimalCodeSecondaryColor.ToColor() ?? Color.FromRgb(255, 165, 0);

        baseTheme.ApplyBaseTheme(primaryColor, secondaryColor);
    }

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
}