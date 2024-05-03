using System.Collections.ObjectModel;

namespace MyExpenses.Models.Sql.Groups;

public class CountryGroup
{
    public string? Country { get; set; }
    public ObservableCollection<CityGroup>? CityGroups { get; set; }
}