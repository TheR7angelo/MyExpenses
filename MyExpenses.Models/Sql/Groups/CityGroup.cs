using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Models.Sql.Groups;

public class CityGroup
{
    public string? City { get; set; }
    public List<TPlace>? Places { get; set; }
}