using MyExpenses.Smartphones.ColorManipulation;

namespace MyExpenses.Smartphones;

public class ThemeManager
{
    // private readonly Dictionary<string, Color> _lightColors;
    // private readonly Dictionary<string, Color> _darkColors;

    public ThemeManager()
    {
        // // Définit les couleurs par défaut pour les thèmes clair et sombre
        // _lightColors = new Dictionary<string, Color>
        // {
        //     {"PrimaryColor", Color.FromArgb("#6200EE")},
        //     {"TextColor", Color.FromArgb("#FFFFFF")}
        // };
        //
        // _darkColors = new Dictionary<string, Color>
        // {
        //     {"PrimaryColor", Color.FromArgb("#BB86FC")},
        //     {"TextColor", Color.FromArgb("#000000")}
        // };
    }

    public void ApplyTheme(AppTheme theme)
    {
        // var themeColors = theme == AppTheme.Dark ? _darkColors : _lightColors;
        // foreach (var color in themeColors)
        // {
        //     Application.Current.Resources[color.Key] = color.Value;
        // }
        //
        // Application.Current.UserAppTheme = theme;
    }

    public void SetPrimaryColor(Color color)
    {
        if (Application.Current is null) return;

        const string primaryLightKey = "PrimaryLight";
        const string primaryMidKey = "PrimaryMid";
        const string primaryDarkKey = "PrimaryDark";

        var primaryLight = color.Lighten();
        var primaryDark = color.Darken();

        // if (Application.Current.Resources.TryGetValue(primaryLightKey, out var z))
        // {
        //     Application.Current.Resources[primaryLightKey] = primaryLight;
        // }

        foreach (var dictionary in Application.Current.Resources.MergedDictionaries)
        {
            if (dictionary.Keys.Contains(primaryLightKey))
            {
                // Mettre à jour les ressources
                dictionary[primaryLightKey] = primaryLight;
                dictionary[primaryMidKey] = color;
                dictionary[primaryDarkKey] = primaryDark;

                // Notifier le changement de propriété si nécessaire
                break;
            }
        }
        //
        // Application.Current.Resources[primaryLightKey] = primaryLight;
        // Application.Current.Resources[primaryMidKey] = color;
        // Application.Current.Resources[primaryDarkKey] = primaryDark;
    }

    // public void SetThemeColor(string key, Color lightColor, Color darkColor)
    // {
    //     // _lightColors[key] = lightColor;
    //     // _darkColors[key] = darkColor;
    //
    //     // Mettre à jour la couleur courante si l'application est actuellement exécutée
    //     var currentTheme = Application.Current.UserAppTheme;
    //     if (currentTheme is AppTheme.Dark)
    //     {
    //         Application.Current.Resources[key] = darkColor;
    //     }
    //     else
    //     {
    //         Application.Current.Resources[key] = lightColor;
    //     }
    // }
}