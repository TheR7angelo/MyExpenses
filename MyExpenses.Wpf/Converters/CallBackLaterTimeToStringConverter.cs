using System.Globalization;
using System.Windows.Data;
using MyExpenses.Models.Wpf.AutoUpdaterGitHub;
using MyExpenses.Wpf.Resources.Resx.Converters.CallBackLaterTimeToStringConverter;

namespace MyExpenses.Wpf.Converters;

public class CallBackLaterTimeToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CallBackLaterTime callBackLaterTime) return string.Empty;

        var str = callBackLaterTime switch
        {
            CallBackLaterTime.After30Minutes => CallBackLaterTimeToStringConverterResources.After30Minutes,
            CallBackLaterTime.After12Hours => CallBackLaterTimeToStringConverterResources.After12Hours,
            CallBackLaterTime.After1Days => CallBackLaterTimeToStringConverterResources.After1Days,
            CallBackLaterTime.After2Days => CallBackLaterTimeToStringConverterResources.After2Days,
            CallBackLaterTime.After4Days => CallBackLaterTimeToStringConverterResources.After4Days,
            CallBackLaterTime.After8Days => CallBackLaterTimeToStringConverterResources.After8Days,
            CallBackLaterTime.After10Days => CallBackLaterTimeToStringConverterResources.After10Days,
            _ => throw new ArgumentOutOfRangeException()
        };

        return str;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}