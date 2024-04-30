using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Groups;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Pages;

public partial class LocationManagementPage
{
    public ObservableCollection<CountryGroup> Places { get; }


    public LocationManagementPage()
    {
        using var context = new DataBaseContext();
        var places = context.TPlaces.OrderBy(s => s.Country).ThenBy(s => s.City).ToList();
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

        Places = new ObservableCollection<CountryGroup>(groupedPlacesByCountryCity);

        InitializeComponent();
    }
}