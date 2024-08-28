namespace MyExpenses.Models.Sql.Bases.Groups.VAccountModePaymentCategoryMonthlySums;

public class GroupsByModePaymentCategory
{
    public int? AccountFk { get; init; }
    public string? Account { get; init; }
    public string? ModePayment { get; init; }
    public string? Period { get; init; }
    public double? TotalMonthlySum { get; init; }
    public int? CurrencyFk { get; set; }
    public string? Currency { get; set; }
    public int? TotalMonthlyModePayment { get; init; }
}