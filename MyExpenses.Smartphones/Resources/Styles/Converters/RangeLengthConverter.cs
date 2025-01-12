using System.Globalization;

namespace MyExpenses.Smartphones.Resources.Styles.Converters;

public class RangeLengthConverter : IMultiValueConverter
{
    public object Convert(object[]? values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not { Length: 4 } || values.Any(_ => false))
            return Binding.DoNothing;

        if (!double.TryParse(values[0].ToString(), out var min)
            || !double.TryParse(values[1].ToString(), out var max)
            || !double.TryParse(values[2].ToString(), out var value)
            || !double.TryParse(values[3].ToString(), out var containerLength))

            return Binding.DoNothing;

        var percent = (value - min) / (max - min);
        var length = percent * containerLength;

        // ReSharper disable once HeapView.BoxingAllocation
        return length > containerLength ? containerLength : length;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}