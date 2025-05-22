using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Bases.Groups;
using MyExpenses.Models.Sql.Bases.Tables;

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
        // ReSharper disable HeapView.ObjectAllocation.Evident
        var groupedPlacesByCountryCity = places
            .GroupBy(country => country.Country)
            .Select(country => new CountryGroup
                {
                Country = country.Key,
                CityGroups = new ObservableCollection<CityGroup>(
                    country.GroupBy(s => s.City)
                        .Select(city => new CityGroup
                        {
                            City = city.Key,
                            Places = new ObservableCollection<TPlace>(city.ToList())
                        }).ToList())
            }).ToList();
        // ReSharper restore HeapView.ObjectAllocation.Evident

        return groupedPlacesByCountryCity;
    }

    public static CountryGroup GetGroups(this TPlace places)
    {
        var countryGroup = new CountryGroup
        {
            Country = places.Country,
            CityGroups =
            [
                new CityGroup
                {
                    City = places.City,
                    Places = [places]
                }
            ]
        };

        return countryGroup;
    }
}