using System.Collections.ObjectModel;
using TPlace = MyExpenses.Models.Sql.Bases.Tables.TPlace;

namespace MyExpenses.Models.Sql.Bases.Groups;

public class CityGroup
{
    public string? City { get; set; }
    public ObservableCollection<TPlace>? Places { get; set; }
}