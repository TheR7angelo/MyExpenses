using CommunityToolkit.Mvvm.Messaging.Messages;
using Domain.Models.Dependencies;

namespace MyExpenses.Presentation.Messages;

public enum DataAction
{
    Add,
    Update,
    Delete
}

public class EntityChangedMessage<T>((DependencyType EntityType, DataAction DataAction, T Content) value)
    : ValueChangedMessage<(DependencyType EntityType, DataAction DataAction, T Content)>(value);