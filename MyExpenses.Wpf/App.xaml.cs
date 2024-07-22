using System.Globalization;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Utils;
using Log = Serilog.Log;
using Theme = MyExpenses.Models.Config.Interfaces.Theme;

namespace MyExpenses.Wpf;


public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var splashScreenWindow = new SplashScreen("Resources\\Assets\\Icon Resize.png");
        splashScreenWindow.Show(true, true);

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

        Log.Information("Apply interface configuration");
        LoadInterfaceConfiguration(configuration.Interface);

        AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
    }

    private static void LoadInterfaceConfiguration(Interface configurationInterface)
    {
        LoadInterfaceTheme(configurationInterface.Theme);
        LoadInterfaceLanguage(configurationInterface.Language);
    }

    public static void LoadInterfaceLanguage(string? cultureInfoCode)
    {
        if (string.IsNullOrEmpty(cultureInfoCode))
        {
            var currentCurrentCulture = CultureInfo.CurrentUICulture.Name;
            using var context = new DataBaseContext(DbContextBackup.LocalFilePathDataBaseModel);
            var supportedLanguages = context.TSupportedLanguages.ToList();

            cultureInfoCode = supportedLanguages.Select(s => s.Code).Contains(currentCurrentCulture)
                ? currentCurrentCulture
                : supportedLanguages.First(s => (bool)s.DefaultLanguage!).Code;
            var configuration = Config.Configuration;
            configuration.Interface.Language = cultureInfoCode;
            configuration.WriteConfiguration();
        }
        
        var cultureInfo = new CultureInfo(cultureInfoCode);
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
    }

    public static void LoadInterfaceTheme(Theme configurationTheme)
    {
        var baseTheme = (BaseTheme)configurationTheme.BaseTheme;
        var primaryColor = configurationTheme.HexadecimalCodePrimaryColor.ToColor() ?? Color.FromRgb(0, 128, 0);
        var secondaryColor = configurationTheme.HexadecimalCodeSecondaryColor.ToColor() ?? Color.FromRgb(255, 165, 0);

        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();
        theme.SetBaseTheme(baseTheme);
        theme.SetPrimaryColor(primaryColor);
        theme.SetSecondaryColor(secondaryColor);

        paletteHelper.SetTheme(theme);
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