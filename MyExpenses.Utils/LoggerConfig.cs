using Serilog;
using Serilog.Core;

namespace MyExpenses.Utils;

public static class LoggerConfig
{
    public static Logger CreateConfig()
    {
        Directory.CreateDirectory("log");

        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(Path.Join("log", $"log-{Guid.NewGuid()}.log"))
            .CreateLogger();
    }
}