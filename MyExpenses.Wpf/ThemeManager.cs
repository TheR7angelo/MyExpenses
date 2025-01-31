using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace MyExpenses.Wpf;

public static class ThemeManager
{
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    private static PaletteHelper ThemeHelper { get; } = new();

    /// <summary>
    /// Retrieves the current theme being used by the application.
    /// </summary>
    /// <returns>The current theme as an instance of the Theme class.</returns>
    private static Theme GetCurrentTheme()
        => ThemeHelper.GetTheme();

    /// <summary>
    /// Changes the application's base theme and optionally updates the primary or secondary color.
    /// If no color is provided, the current color values remain unchanged.
    /// </summary>
    /// <param name="baseTheme">The base theme to set (Light, Dark, or Inherit).</param>
    /// <param name="primaryColor">Optional. If provided, set the theme's primary color.</param>
    /// <param name="secondaryColor">Optional. If provided, set the theme's secondary color.</param>
    public static void ApplyBaseTheme(this BaseTheme baseTheme, Color? primaryColor = null,
        Color? secondaryColor = null)
    {
        var theme = GetCurrentTheme();
        theme.SetBaseTheme(baseTheme);

        if (primaryColor is { } pColor) theme.SetPrimaryColor(pColor);
        if (secondaryColor is { } sColor) theme.SetSecondaryColor(sColor);

        ThemeHelper.SetTheme(theme);
    }
}