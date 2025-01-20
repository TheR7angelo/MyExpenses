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

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // Intentionally initializing the property with a default object to ensure a valid instance
    // even if the JSON does not provide a corresponding value.
    [JsonProperty("theme")]
    public Theme Theme { get; set; } = new();

    [JsonProperty("language")]
    public string? Language { get; set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // Intentionally initializing the property with a default object to ensure a valid instance
    // even if the JSON does not provide a corresponding value.
    [JsonProperty("clock")]
    public Clock Clock { get; set; } = new();
}