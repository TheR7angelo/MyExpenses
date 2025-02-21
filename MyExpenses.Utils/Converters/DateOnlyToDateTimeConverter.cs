using MyExpenses.Utils.DateTimes;

namespace MyExpenses.Utils.Converters;

public static class DateOnlyToDateTimeConverter
{
    public static object? Convert(object? value)
    {
        if (value is DateOnly dateOnly)
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return DateTimeExtensions.ToDateTime(dateOnly);
        }
        return value;
    }

    public static object? ConvertBack(object? value)
    {
        if (value is DateTime dateTime)
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return DateTimeExtensions.ToDateOnly(dateTime);
        }
        return value;
    }
}