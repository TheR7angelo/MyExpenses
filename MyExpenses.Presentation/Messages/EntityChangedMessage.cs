using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyExpenses.Presentation.Messages;

public enum DataAction { Add, Update, Delete }

public enum EntityType { Account }

public class EntityChangedMessage((EntityType EntityType, DataAction DataAction) value) : ValueChangedMessage<(EntityType EntityType, DataAction DataAction)>(value);