using System.Windows;
using Serilog;

namespace MyExpenses.Wpf;


public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Log.Logger = MyExpenses.Utils.LoggerConfig.CreateConfig();
        Log.Information("Démmarrage de l'application");

        AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
    }

    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex) Log.Fatal(ex, "Une erreur inattendue est survenue");
        else Log.Fatal("Une erreur inattendue sans exception associée est survenue");

        Log.CloseAndFlush();
    }

    private static void CurrentDomainOnProcessExit(object? sender, EventArgs e)
    {
        Log.Information("Application quitter");
        Log.CloseAndFlush();
    }
}