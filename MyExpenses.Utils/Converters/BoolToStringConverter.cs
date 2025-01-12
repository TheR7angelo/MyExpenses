using MyExpenses.Utils.Resources.Resx.Converters.BoolToStringConverter;

namespace MyExpenses.Utils.Converters;

public static class BoolToStringConverter
{
    public static string? Convert(object? value)
    {
        if (value is not bool b) return null;

        return b ? BoolToStringConverterResources.Checked : BoolToStringConverterResources.Unchecked;
    }

    public static bool? ConvertBack(object? value)
    {
        if (value is not string s) return null;

        if (s.Equals(BoolToStringConverterResources.Checked))
        {
            return true;
        }

        if (s.Equals(BoolToStringConverterResources.Unchecked))
        {
            return false;
        }
        throw new ArgumentOutOfRangeException();
    }
}