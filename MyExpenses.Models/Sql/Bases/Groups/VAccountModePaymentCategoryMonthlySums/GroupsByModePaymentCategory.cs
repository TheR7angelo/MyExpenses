namespace MyExpenses.Models.Sql.Bases.Groups.VAccountModePaymentCategoryMonthlySums;

public class GroupsByModePaymentCategory
{
    public int? AccountFk { get; init; }
    public string? Account { get; init; }
    public string? ModePayment { get; init; }
    public string? Period { get; init; }
    public double? TotalMonthlySum { get; init; }
    public int? CurrencyFk { get; init; }
    public string? Currency { get; init; }
    public int? TotalMonthlyModePayment { get; init; }
}