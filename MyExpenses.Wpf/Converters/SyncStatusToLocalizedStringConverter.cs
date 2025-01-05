using System.Globalization;
using System.Windows.Data;
using MyExpenses.Models.WebApi.DropBox;
using MyExpenses.Wpf.Resources.Resx.Converters.SyncStatusToLocalizedStringConverter;

namespace MyExpenses.Wpf.Converters;

public class SyncStatusToLocalizedStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SyncStatus syncStatus) return string.Empty;

        var resourceManager = SyncStatusToLocalizedStringConverterResources.ResourceManager!;
        var translated = resourceManager.GetString(syncStatus.ToString()) ?? string.Empty;

        return translated;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}