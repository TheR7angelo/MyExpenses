namespace MyExpenses.Application.Dtos.Systems;

public class PlaceDto
{
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
}