using System.Globalization;

namespace MyExpenses.Smartphones.Resources.Styles.Assists;

public enum Elevation
{
    Dp0,
    Dp1,
    Dp2,
    Dp3,
    Dp4,
    Dp5,
    Dp6,
    Dp7,
    Dp8,
    Dp12,
    Dp16,
    Dp24
}

internal static class ElevationInfo
{
    private static readonly IDictionary<Elevation, Shadow?> ShadowsDictionary;

    static ElevationInfo()
    {
        const string shadowsUri = "Resources/Styles/Controls/ShadowsStyles.xaml";
        var resourceDictionary = new ResourceDictionary { Source = new Uri(shadowsUri, UriKind.Relative) };

        ShadowsDictionary = new Dictionary<Elevation, Shadow?>
        {
            { Elevation.Dp0, null },
            { Elevation.Dp1, resourceDictionary["ElevationShadow1"] as Shadow },
            { Elevation.Dp2, resourceDictionary["ElevationShadow2"] as Shadow },
            { Elevation.Dp3, resourceDictionary["ElevationShadow3"] as Shadow },
            { Elevation.Dp4, resourceDictionary["ElevationShadow4"] as Shadow },
            { Elevation.Dp5, resourceDictionary["ElevationShadow5"] as Shadow },
            { Elevation.Dp6, resourceDictionary["ElevationShadow6"] as Shadow },
            { Elevation.Dp7, resourceDictionary["ElevationShadow7"] as Shadow },
            { Elevation.Dp8, resourceDictionary["ElevationShadow8"] as Shadow },
            { Elevation.Dp12, resourceDictionary["ElevationShadow12"] as Shadow },
            { Elevation.Dp16, resourceDictionary["ElevationShadow16"] as Shadow },
            { Elevation.Dp24, resourceDictionary["ElevationShadow24"] as Shadow }
        };
    }

    public static Shadow? GetDropShadow(Elevation elevation) => ShadowsDictionary[elevation];
}

public static class ElevationAssist
{

    public static readonly BindableProperty ElevationProperty =
        BindableProperty.CreateAttached(
            "Elevation",
            typeof(Elevation),
            typeof(ElevationAssist),
            default(Elevation));

    public static void SetElevation(BindableObject element, Elevation value) => element.SetValue(ElevationProperty, value);
    public static Elevation GetElevation(BindableObject element) => (Elevation)element.GetValue(ElevationProperty);

    public static Shadow? GetDropShadow(Elevation elevation) => ElevationInfo.GetDropShadow(elevation);
}

public class ElevationMarginConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Elevation elevation && elevation != Elevation.Dp0)
        {
            var shadow = ElevationAssist.GetDropShadow(elevation);
            if (shadow != null)
            {
                return new Thickness(shadow.Radius);
            }
        }
        return new Thickness(0);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class ElevationRadiusConverter : IValueConverter
{
    public double Multiplier { get; } = 1.0;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Elevation elevation && elevation != Elevation.Dp0)
        {
            var shadow = ElevationAssist.GetDropShadow(elevation);
            if (shadow != null)
            {
                return shadow.Radius * Multiplier;
            }
        }
        return 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}