using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using BruTile.Predefined;
using MyExpenses.Models.Sql.Groups;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;

namespace MyExpenses.Wpf.Pages;

public partial class LocationManagementPage
{
    public ObservableCollection<CountryGroup> Places { get; }

    public List<KnownTileSource> KnownTileSources { get; }
    public LocationManagementPage()
    {
        KnownTileSources = Enum.GetValues<KnownTileSource>().ToList();

        using var context = new DataBaseContext();
        var places = context.TPlaces.OrderBy(s => s.Country).ThenBy(s => s.City).ToList();
        var groups = places.GetGroups();

        Places = new ObservableCollection<CountryGroup>(groups);

        InitializeComponent();
    }

    private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (sender is not TreeView treeView) return;

        if(treeView.SelectedItem is not TPlace place) return;

        Console.WriteLine(place.Name);
    }
}