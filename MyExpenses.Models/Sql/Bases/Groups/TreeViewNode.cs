using System.Collections.ObjectModel;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Groups;

[AddINotifyPropertyChangedInterface]
public sealed class TreeViewNode
{
    public string? Name { get; init; }
    public IList<TreeViewNode> Children { get; init; } = new ObservableCollection<TreeViewNode>();

    public object? AdditionalData { get; set; }
}