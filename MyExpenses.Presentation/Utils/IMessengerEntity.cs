using CommunityToolkit.Mvvm.Messaging;

namespace MyExpenses.Presentation.Utils;

public interface IMessengerEntity
{
    void UnRegister() => WeakReferenceMessenger.Default.UnregisterAll(this);
}