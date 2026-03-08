namespace Domain.Models.Accounts;

public class TotalByAccountDomain
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public double Total { get; set; }

    public double TotalPointed { get; set; }

    public double TotalNotPointed { get; set; }

    public required string Symbol { get; set; }
}