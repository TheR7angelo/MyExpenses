using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace MyExpenses.Smartphones.Converters;

public class MaxLengthConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is null) return 0;

        var objectType = value.GetType();
        var propertyName = parameter.ToString()!;

        var property = objectType.GetProperty(propertyName);

        var maxLengthAttribute = property?.GetCustomAttribute<MaxLengthAttribute>();
        return maxLengthAttribute?.Length ?? int.MaxValue;

    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}