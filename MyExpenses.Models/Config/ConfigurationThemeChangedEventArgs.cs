using MyExpenses.Models.Config.Interfaces;

namespace MyExpenses.Models.Config;

public class ConfigurationThemeChangedEventArgs(Theme newTheme) : EventArgs
{
    public Theme Theme { get; private set; } = newTheme;
}