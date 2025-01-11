using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace MyExpenses.Smartphones.Converters;

public class MaxLengthConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is null)
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return 0;
        }

        var objectType = value.GetType();
        var propertyName = parameter.ToString()!;

        // ReSharper disable once HeapView.BoxingAllocation
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