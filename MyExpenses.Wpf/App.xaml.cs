using System.Windows;
using Serilog;

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
        var logMaxDays = configuration.Log.MaxDaysLog;
        MyExpenses.Utils.LoggerConfig.RemoveOldLog(logMaxDays);

        AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
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