using CsvHelper.Configuration.Attributes;

namespace MyExpenses.Models.IO.Sig.Shp;

public class GeographicCoordinateSystem
{
    [Name("Well-known ID")]
    public int WellKnownId { get; init; }

    [Name("Name")]
    public string Name { get; init; } = string.Empty;

    [Name("prj")]
    public string Prj { get; init; } = string.Empty;
}