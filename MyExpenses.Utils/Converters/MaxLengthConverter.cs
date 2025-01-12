using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MyExpenses.Utils.Converters;

public static class MaxLengthConverter
{
    public static object? Convert(object? value, object? parameter)
    {
        // ReSharper disable once HeapView.BoxingAllocation
        if (value is null || parameter is null) return 0;

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
}