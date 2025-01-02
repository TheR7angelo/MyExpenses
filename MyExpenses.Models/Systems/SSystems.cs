using Serilog.Events;

namespace MyExpenses.Models.Systems;

public struct SSystems
{
    public EEnvironmentType? EnvironmentType { get; set; }
    public LogEventLevel? LogEventLevel { get; set; }
}