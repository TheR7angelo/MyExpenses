using System.Collections.ObjectModel;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Groups;

[AddINotifyPropertyChangedInterface]
public sealed class TreeViewNode
{
    public string? Name { get; init; }
    // The Children list is initialized by default to avoid null references
    // and ensure the property is ready to use out of the box.
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public IList<TreeViewNode> Children { get; init; } = new ObservableCollection<TreeViewNode>();

    public object? AdditionalData { get; set; }
}