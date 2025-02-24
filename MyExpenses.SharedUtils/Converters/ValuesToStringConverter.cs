using System.Globalization;
using MyExpenses.SharedUtils.Resources.Resx.PopupFilterManagement;

namespace MyExpenses.SharedUtils.Converters;

public static class ValuesToStringConverter
{
    public static string Convert(this object?[] values, CultureInfo culture)
    {
        foreach (var value in values)
        {
            switch (value)
            {
                case null:
                    continue;
                case string str:
                    return str;
                case int i:
                    return i.ToString();
                case decimal d:
                    return d.ToString(culture);
                case double dbl:
                    return dbl.ToString(culture);
                case bool b:
                    return b ? PopupFilterManagementResources.Checked : PopupFilterManagementResources.Unchecked;
            }
        }

        return string.Empty;
    }
}