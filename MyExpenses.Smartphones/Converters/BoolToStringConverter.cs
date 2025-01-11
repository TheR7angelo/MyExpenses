using System.Globalization;
using MyExpenses.Smartphones.Resources.Resx.Converters.BoolToStringConverter;

namespace MyExpenses.Smartphones.Converters;

public class BoolToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool b) return Binding.DoNothing;

        return b ? BoolToStringConverterResources.Checked : BoolToStringConverterResources.Unchecked;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string s) return Binding.DoNothing;

        if (s.Equals(BoolToStringConverterResources.Checked))
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return true;
        }

        if (s.Equals(BoolToStringConverterResources.Unchecked))
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return false;
        }
        throw new ArgumentOutOfRangeException();
    }
}