using System.Globalization;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MyExpenses.Presentation.Messages;

public class LanguageChangedMessage(CultureInfo value) : ValueChangedMessage<CultureInfo>(value);
