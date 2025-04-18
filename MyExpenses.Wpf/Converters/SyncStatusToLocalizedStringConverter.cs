using System.Globalization;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class SyncStatusToLocalizedStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => MyExpenses.Utils.Converters.SyncStatusToLocalizedStringConverter.Convert(value);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => MyExpenses.Utils.Converters.SyncStatusToLocalizedStringConverter.ConvertBack(value);
}