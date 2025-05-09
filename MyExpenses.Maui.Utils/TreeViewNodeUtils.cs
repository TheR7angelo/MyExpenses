using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Bases.Groups;
using MyExpenses.Utils.Converters;

namespace MyExpenses.Maui.Utils;

public static class TreeViewNodeUtils
{
    /// <summary>
    /// Converts an array of <see cref="CountryGroup"/> objects into a list of <see cref="TreeViewNode"/> objects.
    /// </summary>
    /// <param name="countriesGroups">An array of <see cref="CountryGroup"/> representing the hierarchical structure to be transformed into tree view nodes.</param>
    /// <returns>A list of <see cref="TreeViewNode"/> objects representing the hierarchical tree structure.</returns>
    public static List<TreeViewNode> ToTreeViewNode(this CountryGroup[] countriesGroups)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var treeViewNodes = new List<TreeViewNode>();

        foreach (var countryGroup in countriesGroups)
        {
            var firstChildren = countryGroup.ToTreeViewNode();
            var countryName = countryGroup.Country.FormatNodeName(firstChildren);

            var item = countryName.CreateTreeViewNode(firstChildren);
            item.AdditionalData = countryGroup.Country;
            treeViewNodes.Add(item);
        }

        return treeViewNodes;
    }

    private static List<TreeViewNode> ToTreeViewNode(this CountryGroup countryGroup)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var treeViewNodes = new List<TreeViewNode>();
        if (countryGroup.CityGroups is null) return treeViewNodes;

        for (var i = 0; i < countryGroup.CityGroups.Count; i++)
        {
            var cityGroup = countryGroup.CityGroups[i];
            var secondChildren = cityGroup.ToTreeViewNode();
            var cityName = cityGroup.City.FormatNodeName(secondChildren);

            var item = cityName.CreateTreeViewNode(secondChildren);
            item.AdditionalData = cityGroup.City;
            treeViewNodes.Add(item);
        }

        return treeViewNodes;
    }

    private static List<TreeViewNode> ToTreeViewNode(this CityGroup cityGroup)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var treeViewNodes = new List<TreeViewNode>();
        if (cityGroup.Places is null) return treeViewNodes;

        for (var i = 0; i < cityGroup.Places.Count; i++)
        {
            var place = cityGroup.Places[i];
            var placeName = EmptyStringTreeViewConverter.ToUnknown(place.Name);
            var item = placeName.CreateTreeViewNode(additionalData: place);
            treeViewNodes.Add(item);
        }

        return treeViewNodes;
    }

    /// <summary>
    /// Formats the name of a node by appending the count of associated <see cref="TreeViewNode"/> objects.
    /// </summary>
    /// <param name="rawName">The raw name of the node as a string, which might be null or empty.</param>
    /// <param name="treeViewNodes">A list of <see cref="TreeViewNode"/> objects associated with the node.</param>
    /// <returns>A formatted string containing the node name and the count of associated <see cref="TreeViewNode"/> objects.</returns>
    private static string FormatNodeName(this string? rawName, List<TreeViewNode> treeViewNodes)
    {
        var name = EmptyStringTreeViewConverter.ToUnknown(rawName);
        return $"{name} [{treeViewNodes.Count}]";
    }

    /// <summary>
    /// Creates a new instance of <see cref="TreeViewNode"/> with the specified name, optional children, and additional data.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <param name="children">A list of child nodes for this node. Defaults to an empty list if not specified.</param>
    /// <param name="additionalData">Additional data associated with the node. Can be null if no data is provided.</param>
    /// <returns>A new <see cref="TreeViewNode"/> instance populated with the provided parameters.</returns>
    private static TreeViewNode CreateTreeViewNode(this string name, List<TreeViewNode>? children = null,
        object? additionalData = null)
    {
        children ??= [];
        var collection = new ObservableCollection<TreeViewNode>(children);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        return new TreeViewNode
        {
            Name = name,
            Children = collection,
            AdditionalData = additionalData
        };
    }
}