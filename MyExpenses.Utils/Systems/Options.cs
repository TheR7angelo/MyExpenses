using CommandLine;
using Serilog.Events;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MyExpenses.Utils.Systems;

public sealed class Options
{
    [Option( 'd', "debugLevel", Required = false, HelpText = "Debug level")]
    public LogEventLevel DebugLevel { get; set; }

    [Option( 'l', "logEfCore", Required = false, HelpText = "Log EF Core" )]
    public bool LogEfCore { get; set; }

    [Option( 'w', "outputEfCoreLog", Required = false, HelpText = "Specifies whether EF Core logs should be written to a file.")]
    public bool WriteToFileEfCore { get; set; }
}