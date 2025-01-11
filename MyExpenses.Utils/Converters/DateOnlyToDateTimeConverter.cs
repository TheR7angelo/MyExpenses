using MyExpenses.Utils.DateTimes;

namespace MyExpenses.Utils.Converters;

public static class DateOnlyToDateTimeConverter
{
    public static object? Convert(this object? value)
    {
        if (value is DateTime dateTime)
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
        }
        return value;
    }

    public static object? ConvertBack(this object? value)
    {
        if (value is DateTime dateTime)
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return DateTimeExtensions.ToDateOnly(dateTime);
        }
        return value;
    }
}