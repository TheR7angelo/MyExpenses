using System.Globalization;
using System.Windows.Data;
using Domain.Models.Dependencies;

namespace MyExpenses.Wpf.Converters;

public class DependencyToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not EntityType dependencyType) return null;
        var iconName = dependencyType.GetRessourceIconName();

        return System.Windows.Application.Current.TryFindResource(iconName);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}