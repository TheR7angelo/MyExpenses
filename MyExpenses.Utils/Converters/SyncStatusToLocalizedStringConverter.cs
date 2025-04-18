using MyExpenses.Models.Utils;
using MyExpenses.Models.WebApi.DropBox;
using MyExpenses.Utils.Resources.Resx.Converters.SyncStatusToLocalizedStringConverter;

namespace MyExpenses.Utils.Converters;

public static class SyncStatusToLocalizedStringConverter
{
    public static object Convert(this object? value)
    {
        if (value is not SyncStatus syncStatus) return string.Empty;

        var resourceManager = SyncStatusToLocalizedStringConverterResources.ResourceManager!;
        var name = EnumHelper<SyncStatus>.ToEnumString(syncStatus);
        var translated = resourceManager.GetString(name) ?? string.Empty;

        return translated;
    }

    public static object ConvertBack(object? value)
    {
        throw new NotImplementedException();
    }
}