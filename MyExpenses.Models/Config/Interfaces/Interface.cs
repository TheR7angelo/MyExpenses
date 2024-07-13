using Newtonsoft.Json;

namespace MyExpenses.Models.Config.Interfaces;

public class Interface
{
    public delegate void ThemeChangedEventHandler(object sender, ConfigurationThemeChangedEventArgs e);

    public static event ThemeChangedEventHandler? ThemeChanged;

    public static void OnThemeChanged(object sender, ConfigurationThemeChangedEventArgs e)
    {
        ThemeChanged?.Invoke(sender, e);
    }

    public delegate void LanguageChangedEventHandler(object sender, ConfigurationLanguageChangedEventArgs e);

    public static event LanguageChangedEventHandler? LanguageChanged;

    public static void OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
    {
        LanguageChanged?.Invoke(sender, e);
    }

    [JsonProperty("theme")]
    public Theme Theme { get; set; } = new();

    [JsonProperty("language")]
    public string? Language { get; set; } = "en-001";

    [JsonProperty("clock")]
    public Clock Clock { get; set; } = new();
}