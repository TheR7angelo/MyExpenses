namespace MyExpenses.Models.Config;

public class ConfigurationChangedEventArgs(Configuration newConfiguration) : EventArgs
{
    public Configuration Configuration { get; private set; } = newConfiguration;
}