using Newtonsoft.Json;

namespace MyExpenses.Models.Config.Interfaces;

public class Interface
{
    public delegate void ThemeChangedEventHandler();

    public static event ThemeChangedEventHandler? ThemeChanged;

    public static void OnThemeChanged()
    {
        ThemeChanged?.Invoke();
    }

    public delegate void LanguageChangedEventHandler();

    public static event LanguageChangedEventHandler? LanguageChanged;

    public static void OnLanguageChanged()
    {
        LanguageChanged?.Invoke();
    }

    [JsonProperty("theme")]
    public Theme Theme { get; set; } = new();

    [JsonProperty("language")]
    public string? Language { get; set; }

    [JsonProperty("clock")]
    public Clock Clock { get; set; } = new();
}