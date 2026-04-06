using CommunityToolkit.Mvvm.Messaging.Messages;
using Domain.Models.Dependencies;

namespace MyExpenses.Presentation.Messages;

public enum DataAction
{
    Add,
    Update,
    Delete
}

public class EntityChangedMessage<T>((EntityType EntityType, DataAction DataAction, T Content) value)
    : ValueChangedMessage<(EntityType EntityType, DataAction DataAction, T Content)>(value);