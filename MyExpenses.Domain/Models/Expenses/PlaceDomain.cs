namespace Domain.Models.Expenses;

public class PlaceDomain
{
    public const int MaxNameLength = 155;
    public const int MaxNumberLength = 20;
    public const int MaxStreetLength = 155;
    public const int MaxPostalLength = 10;
    public const int MaxCityLength = 100;
    public const int MaxCountryLength = 55;

    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Number { get; set; }

    public required string Street { get; set; }

    public required string Postal { get; set; }

    public required string City { get; set; }

    public required string Country { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public bool IsOpen { get; set; } = true;

    public bool CanBeDeleted { get; init; } = true;

    public DateTime DateAdded { get; set; } = DateTime.Now;

    public HistoryDomain[] Histories { get; set; } = [];

    public RecursiveExpenseDomain[] RecursiveExpenses { get; set; } = [];
}