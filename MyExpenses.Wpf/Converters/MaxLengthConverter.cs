using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class MaxLengthConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is null) return 0;

        var objectType = value.GetType();
        var propertyName = parameter.ToString()!;

        return Convert(objectType, propertyName);
    }

    public static int Convert(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName);

        var maxLengthAttribute = property?.GetCustomAttribute<MaxLengthAttribute>();
        return maxLengthAttribute?.Length ?? int.MaxValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}