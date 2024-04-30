using MyExpenses.Models.Sql.Groups;
using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Utils;

public static class GroupExtensions
{
    public static IEnumerable<CountryGroup> GetGroups(this IEnumerable<TPlace> places)
    {
        var groupedPlacesByCountryCity = places
            .GroupBy(country => country.Country)
            .Select(country => new CountryGroup
            {
                Country = country.Key ?? "Unknown",
                CityGroups = country.GroupBy(s => s.City)
                    .Select(city => new CityGroup
                    {
                        City = city.Key ?? "Unknown",
                        Places = city.ToList()
                    }).ToList()
            }).ToList();
        return groupedPlacesByCountryCity;
    }
}