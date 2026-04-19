using CommunityToolkit.Mvvm.Messaging.Messages;
using Domain.Models.Dependencies;

namespace MyExpenses.Presentation.Messages;

public enum DataAction
{
    Add,
    Update,
    Delete
}

public class EntityChangedMessage<T>(EntityChanged<T> value)
    : ValueChangedMessage<EntityChanged<T>>(value);

public readonly struct EntityChanged<T>
{
    public DependencyType EntityType { get; init; }
    public DataAction DataAction { get; init; }
    public T Content { get; init; }
}