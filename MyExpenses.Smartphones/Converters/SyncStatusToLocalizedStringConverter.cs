using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

public class SyncStatusToLocalizedStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Utils.Converters.SyncStatusToLocalizedStringConverter.Convert(value);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Utils.Converters.SyncStatusToLocalizedStringConverter.ConvertBack(value);
}