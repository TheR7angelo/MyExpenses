using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Groups;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;

namespace MyExpenses.Wpf.Pages;

public partial class LocationManagementPage
{
    public ObservableCollection<CountryGroup> Places { get; }


    public LocationManagementPage()
    {
        using var context = new DataBaseContext();
        var places = context.TPlaces.OrderBy(s => s.Country).ThenBy(s => s.City).ToList();
        var groups = places.GetGroups();

        Places = new ObservableCollection<CountryGroup>(groups);

        InitializeComponent();
    }
}