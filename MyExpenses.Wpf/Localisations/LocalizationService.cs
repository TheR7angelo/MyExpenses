using System.Globalization;

namespace MyExpenses.Wpf.Localisations;

public class LocalizationService
{
    public static LocalizationService Instance { get; } = new();

    public event EventHandler? LanguageChanged;

    public CultureInfo CurrentCulture { get; private set; }
        = CultureInfo.CurrentUICulture;

    public void SetLanguage(string culture)
    {
        CurrentCulture = new CultureInfo(culture);
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}