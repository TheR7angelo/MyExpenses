using System.Collections.ObjectModel;

namespace MyExpenses.Models.Sql.Bases.Groups;

public class CountryGroup
{
    public string? Country { get; set; }
    public ObservableCollection<CityGroup>? CityGroups { get; set; }
}