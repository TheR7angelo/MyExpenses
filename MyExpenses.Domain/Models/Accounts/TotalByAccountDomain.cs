namespace Domain.Models.Accounts;

public class TotalByAccountDomain
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public double Total { get; set; }

    public double TotalPointed { get; set; }

    public double TotalNotPointed { get; set; }

    public string Symbol { get; set; } = string.Empty;
}