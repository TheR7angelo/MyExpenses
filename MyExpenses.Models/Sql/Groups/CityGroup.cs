using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Models.Sql.Groups;

public class CityGroup
{
    public string? City { get; set; }
    public ObservableCollection<TPlace>? Places { get; set; }
}