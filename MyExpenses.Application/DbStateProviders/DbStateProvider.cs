namespace MyExpenses.Application.DbStateProviders;

public class DbStateProvider : IDbStateProvider
{
    public string? FilePath { get; set; }
}