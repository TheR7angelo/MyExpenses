using CommandLine;
using MyExpenses.Models.Systems;
using Serilog.Events;

namespace MyExpenses.Utils.Systems;

public sealed class Options
{
    [Option( 'e', "environment", Required = false, HelpText = "Environment to run")]
    public EEnvironmentType Environment { get; set; }

    [Option( 'd', "debuglevel", Required = false, HelpText = "Debug level")]
    public LogEventLevel DebugLevel { get; set; }
}