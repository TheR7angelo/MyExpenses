namespace MyExpenses.Models.Config;

public class ConfigurationLanguageChangedEventArgs(string newCultureInfoCode) : EventArgs
{
    public string CultureInfoCode { get; private set; } = newCultureInfoCode;
}