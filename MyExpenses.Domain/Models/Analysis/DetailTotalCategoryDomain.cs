namespace Domain.Models.Analysis;

public class DetailTotalCategoryDomain
{
    public const int MaxCategoryLength = 55;

    public int Year { get; init; }

    public int Week { get; init; }

    public int Month { get; init; }

    public int Day { get; init; }

    public string AccountId { get; init; }

    public string AccountName { get; init; }

    public string CategoryName { get; init; }

    public double Value { get; init; }

    public string Symbol { get; init; }

    public string HexadecimalColorCode { get; init; }
}