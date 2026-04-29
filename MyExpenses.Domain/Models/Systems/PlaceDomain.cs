using Domain.Models.Expenses;

namespace Domain.Models.Systems;

public class PlaceDomain
{
    public const int MaxNameLength = 155;
    public const int MaxNumberLength = 20;
    public const int MaxStreetLength = 155;
    public const int MaxPostalLength = 10;
    public const int MaxCityLength = 100;
    public const int MaxCountryLength = 55;

    public const int DefaultPlaceId = 1;

    public int Id { get; set; }

    public string Name { get; set; }

    public string Number { get; set; }

    public string Street { get; set; }

    public string Postal { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public bool IsOpen { get; set; } = true;

    public bool CanBeDeleted { get; init; } = true;

    public DateTime DateAdded { get; set; } = DateTime.Now;
}