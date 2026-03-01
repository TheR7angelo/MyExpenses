namespace MyExpenses.Application.DbStateProviders;

public interface IDbStateProvider
{
    public string CurrentConnectionString { get; set; }
}