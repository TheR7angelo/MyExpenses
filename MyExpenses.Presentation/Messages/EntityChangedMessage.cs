using CommunityToolkit.Mvvm.Messaging.Messages;
using Domain.Models.Dependencies;

namespace MyExpenses.Presentation.Messages;

public enum DataAction { Add, Update, Delete }

public class EntityChangedMessage((EntityType EntityType, DataAction DataAction) value) : ValueChangedMessage<(EntityType EntityType, DataAction DataAction)>(value);