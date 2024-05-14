using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using MyExpenses.Models.Config;
using MyExpenses.Wpf.Utils;
using Log = Serilog.Log;

namespace MyExpenses.Wpf;


public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var splashScreenWindow = new SplashScreen("Resources\\Assets\\Icon Resize.png");
        splashScreenWindow.Show(true, true);

        Log.Logger = MyExpenses.Utils.LoggerConfig.CreateConfig();
        Log.Information("Starting the application");

        Log.Information("Reading configuration file");
        var configuration = MyExpenses.Utils.Config.Configuration;

        Log.Information("Apply log configuration");
        LoadLogConfiguration(configuration.Log);

        Log.Information("Apply interface configuration");
        LoadInterfaceConfiguration(configuration.Interface);

        AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
    }

    private static void LoadInterfaceConfiguration(Interface configurationInterface)
    {
        var baseThemeStr = configurationInterface.BaseTheme;
        if (!Enum.TryParse<BaseTheme>(baseThemeStr, true, out var baseTheme))
        {
            baseTheme = BaseTheme.Inherit;
        }

        var primaryColor = configurationInterface.HexadecimalCodePrimaryColor.ToColor() ?? Color.FromRgb(0, 128, 0);
        var secondaryColor = configurationInterface.HexadecimalCodeSecondaryColor.ToColor() ?? Color.FromRgb(255, 165, 0);

        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();
        theme.SetBaseTheme(baseTheme);
        theme.SetPrimaryColor(primaryColor);
        theme.SetSecondaryColor(secondaryColor);

        paletteHelper.SetTheme(theme);
    }

    private static void LoadLogConfiguration(Models.Config.Log logConfiguration)
    {
        var logMaxDays = logConfiguration.MaxDaysLog;
        MyExpenses.Utils.LoggerConfig.RemoveOldLog(logMaxDays);
    }

    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex) Log.Fatal(ex, "An unexpected error occurred");
        else Log.Fatal("An unexpected error with no associated exception occurred");

        Log.CloseAndFlush();
    }

    private static void CurrentDomainOnProcessExit(object? sender, EventArgs e)
    {
        Log.Information("Application exit");
        Log.CloseAndFlush();
    }
}