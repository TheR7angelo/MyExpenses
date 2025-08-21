using System.Collections.ObjectModel;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Groups;

[AddINotifyPropertyChangedInterface]
public sealed class TreeViewNode
{
    public string? Name { get; set; }
    // The Children list is initialized by default to avoid null references
    // and ensure the property is ready to use out of the box.
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public ObservableCollection<TreeViewNode> Children { get; init; } = [];

    public bool IsLeaf => Children.Count is 0;

    public object? AdditionalData { get; set; }
}