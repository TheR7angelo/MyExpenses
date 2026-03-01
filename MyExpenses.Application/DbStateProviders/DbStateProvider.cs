namespace MyExpenses.Application.DbStateProviders;

public class DbStateProvider : IDbStateProvider
{
    public required string CurrentConnectionString { get; set; }
}