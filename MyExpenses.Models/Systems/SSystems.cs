using Serilog.Events;

namespace MyExpenses.Models.Systems;

public struct SSystems
{
    public LogEventLevel? LogEventLevel { get; set; }
    public bool LogEfCore { get; set; }
    public bool WriteToFileEfCore { get; set; }
}