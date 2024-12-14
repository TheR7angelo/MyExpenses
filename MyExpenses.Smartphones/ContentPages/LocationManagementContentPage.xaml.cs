using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Bases.Groups;
using MyExpenses.Smartphones.Converters;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;

namespace MyExpenses.Smartphones.ContentPages;

public partial class LocationManagementContentPage
{
    public ObservableCollection<TreeViewNode> TreeViewNodes { get; }

    public LocationManagementContentPage()
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

        TreeViewNodes = [..treeViewNodes];

        InitializeComponent();
    }
}