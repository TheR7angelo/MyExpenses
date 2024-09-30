namespace MyExpenses.Smartphones;

public class ThemeManager
{
    private readonly Dictionary<string, Color> _lightColors;
    private readonly Dictionary<string, Color> _darkColors;

    public ThemeManager()
    {
        // Définit les couleurs par défaut pour les thèmes clair et sombre
        _lightColors = new Dictionary<string, Color>
        {
            {"PrimaryColor", Color.FromArgb("#6200EE")},
            {"TextColor", Color.FromArgb("#FFFFFF")}
        };

        _darkColors = new Dictionary<string, Color>
        {
            {"PrimaryColor", Color.FromArgb("#BB86FC")},
            {"TextColor", Color.FromArgb("#000000")}
        };
    }

    public void ApplyTheme(AppTheme theme)
    {
        var themeColors = theme == AppTheme.Dark ? _darkColors : _lightColors;
        foreach (var color in themeColors)
        {
            Application.Current.Resources[color.Key] = color.Value;
        }

        Application.Current.UserAppTheme = theme;
    }

    public void SetThemeColor(string key, Color lightColor, Color darkColor)
    {
        _lightColors[key] = lightColor;
        _darkColors[key] = darkColor;

        // Mettre à jour la couleur courante si l'application est actuellement exécutée
        var currentTheme = Application.Current.UserAppTheme;
        if (currentTheme is AppTheme.Dark)
        {
            Application.Current.Resources[key] = darkColor;
        }
        else
        {
            Application.Current.Resources[key] = lightColor;
        }
    }
}