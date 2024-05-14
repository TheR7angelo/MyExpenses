using System.Windows;
using MyExpenses.Models.Config;
using Log = Serilog.Log;

namespace MyExpenses.Wpf;


public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

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

    private void LoadInterfaceConfiguration(Interface configurationInterface)
    {
        // TODO work
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