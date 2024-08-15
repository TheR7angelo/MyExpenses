using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Models.Sql.Bases.Groups;

public class CityGroup
{
    public string? City { get; set; }
    public ObservableCollection<TPlace>? Places { get; set; }
}