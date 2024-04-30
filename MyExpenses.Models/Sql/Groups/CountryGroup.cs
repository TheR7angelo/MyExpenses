namespace MyExpenses.Models.Sql.Groups;

public class CountryGroup
{
    public string? Country { get; set; }
    public List<CityGroup>? CityGroups { get; set; }
}