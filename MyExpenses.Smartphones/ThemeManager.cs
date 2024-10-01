using MyExpenses.Smartphones.ColorManipulation;

namespace MyExpenses.Smartphones;

public class ThemeManager
{
    public void SetPrimaryColor(Color color)
    {
        if (Application.Current is null) return;

        const string primaryLightKey = "PrimaryLight";
        const string primaryMidKey = "PrimaryMid";
        const string primaryDarkKey = "PrimaryDark";

        var primaryLight = color.Lighten();
        var primaryDark = color.Darken();

        Application.Current.Resources[primaryLightKey] = primaryLight;
        Application.Current.Resources[primaryMidKey] = color;
        Application.Current.Resources[primaryDarkKey] = primaryDark;
    }
}