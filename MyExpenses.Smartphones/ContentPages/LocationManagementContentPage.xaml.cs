using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Bases.Groups;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Smartphones.Converters;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;

namespace MyExpenses.Smartphones.ContentPages;

public partial class LocationManagementContentPage
{
    public ObservableCollection<TreeViewNode> TreeViewNodes { get; }

    public LocationManagementContentPage()
    {
        var (treeViewNodes, places) = GenerateTreeViewNodes();

        TreeViewNodes = [..treeViewNodes];

        InitializeComponent();
        UpdateDisplay();

        DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_OnMainDisplayInfoChanged;
    }

    private void DeviceDisplay_OnMainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
        => UpdateDisplay();

    private void UpdateDisplay()
    {
        foreach (var view in new List<View> { ScrollViewTreeView, Image })
        {
            if (view.Parent is Grid grid) grid.Children.Remove(view);
        }

        var orientation = DeviceDisplay.MainDisplayInfo.Orientation;
        if (orientation is DisplayOrientation.Landscape)
        {
            AddToGrid(GridLandscape, ScrollViewTreeView, 0, 0);
            AddToGrid(GridLandscape, Image, 0, 1);
        }
        else
        {
            AddToGrid(GridPortrait, Image, 0, 0);
            AddToGrid(GridPortrait, ScrollViewTreeView, 1, 0);
        }
    }

    private static void AddToGrid(Grid grid, View control, int row, int column)
    {
        grid.Children.Add(control);
        Grid.SetRow(control, row);
        Grid.SetColumn(control, column);
    }

    private static (IEnumerable<TreeViewNode> TreeViewNodes, IEnumerable<TPlace> Places) GenerateTreeViewNodes()
    {
        using var context = new DataBaseContext();
        var places = context.TPlaces.OrderBy(s => s.Country).ThenBy(s => s.City).ThenBy(s => s.Name).ToList();
        var groups = places.GetGroups();

        var treeViewNodes = new List<TreeViewNode>();
        foreach (var group in groups)
        {
            var firstChildren = new List<TreeViewNode>();
            foreach (var cityGroup in group.CityGroups!)
            {
                var secondChildren = new List<TreeViewNode>();
                foreach (var place in cityGroup.Places!)
                {
                    var placeName = EmptyStringTreeViewConverter.ToUnknown(place.Name);
                    var placeNode = new TreeViewNode { Name = placeName, AdditionalData = place };
                    secondChildren.Add(placeNode);
                }

                var cityName = EmptyStringTreeViewConverter.ToUnknown(cityGroup.City);
                cityName = $"{cityName} [{secondChildren.Count}]";

                var cityNode = new TreeViewNode { Name = cityName, Children = secondChildren };
                firstChildren.Add(cityNode);
            }

            var countryName = EmptyStringTreeViewConverter.ToUnknown(group.Country);
            countryName = $"{countryName} [{firstChildren.Count}]";

            var groupNode = new TreeViewNode { Name = countryName, Children = firstChildren };
            treeViewNodes.Add(groupNode);
        }

        return (treeViewNodes, places);
    }
}