using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MyExpenses.Models.Sql.Bases.Groups;

public sealed class TreeViewNode : ObservableObject
{
    public string? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<TreeViewNode> Children { get; init; } = [];

    public bool IsLeaf => Children.Count is 0;

    public object? AdditionalData
    {
        get;
        set => SetProperty(ref field, value);
    }

    public TreeViewNode()
    {
        Children.CollectionChanged += (s, e) => OnPropertyChanged(nameof(IsLeaf));
    }
}