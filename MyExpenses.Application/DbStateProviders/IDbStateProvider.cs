namespace MyExpenses.Application.DbStateProviders;

public interface IDbStateProvider
{
    public string? FilePath { get; set; }
}