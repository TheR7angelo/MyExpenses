using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

public class BoolToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Utils.Converters.BoolToStringConverter.Convert(value);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        // ReSharper disable once HeapView.BoxingAllocation
        => Utils.Converters.BoolToStringConverter.ConvertBack(value);
}