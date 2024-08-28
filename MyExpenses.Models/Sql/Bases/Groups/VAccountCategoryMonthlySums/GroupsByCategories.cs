namespace MyExpenses.Models.Sql.Bases.Groups.VAccountCategoryMonthlySums;

public class GroupsByCategories
{
    public int? AccountFk { get; init; }
    public string? Account { get; init; }
    public string? Period { get; init; }
    public string? CategoryType { get; init; }
    public string? ColorCode { get; init; }
    public double? SumMonthlySum { get; init; }
    public int? CurrencyFk { get; set; }
    public string? Currency { get; set; }
}