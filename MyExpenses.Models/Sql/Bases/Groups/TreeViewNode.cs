using System.Collections.ObjectModel;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Groups;

[AddINotifyPropertyChangedInterface]
public class TreeViewNode
{
    public virtual string? Name { get; init; }
    public virtual IList<TreeViewNode> Children { get; init; } = new ObservableCollection<TreeViewNode>();

    public object? AdditionalData { get; set; }
}