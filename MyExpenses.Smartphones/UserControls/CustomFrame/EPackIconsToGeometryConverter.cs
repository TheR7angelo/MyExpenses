using System.Globalization;
using MyExpenses.Smartphones.PackIcons;

namespace MyExpenses.Smartphones.UserControls.CustomFrame;

public class EPackIconsToGeometryConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not EPackIcons ePackIcons) return null;
        var geometry = ePackIcons.ToGeometry();

        return geometry;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}