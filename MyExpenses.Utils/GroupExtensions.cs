using MyExpenses.Models.Sql.Groups;
using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Utils;

/// <summary>
/// Provides extension methods for grouping TPlace objects.
/// </summary>
public static class GroupExtensions
{
    /// <summary>
    /// This method groups a collection of TPlace objects by country and city.
    /// </summary>
    /// <param name="places">The collection of TPlace objects to be grouped.</param>
    /// <returns>A collection of CountryGroup objects representing the grouped places.</returns>
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