using MyExpenses.SharedUtils.Resources.Resx.PopupFilterManagement;

namespace MyExpenses.Utils.Converters;

public static class BoolToStringConverter
{
    public static string? Convert(object? value)
    {
        if (value is not bool b) return null;

        return b ? PopupFilterManagementResources.Checked : PopupFilterManagementResources.Unchecked;
    }

    public static bool? ConvertBack(object? value)
    {
        if (value is not string s) return null;

        if (s.Equals(PopupFilterManagementResources.Checked))
        {
            return true;
        }

        if (s.Equals(PopupFilterManagementResources.Unchecked))
        {
            return false;
        }
        throw new ArgumentOutOfRangeException();
    }
}