using MyExpenses.Models.Sql.Bases.Enums;

namespace MyExpenses.Utils.Dates;

public static class DateExtensions
{
    /// <summary>
    /// Converts a DateOnly object to a DateTime object.
    /// </summary>
    /// <param name="dateOnly">The DateOnly object to be converted.</param>
    /// <returns>The corresponding DateTime object with the same date.</returns>
    public static DateTime? ToDateTime(this DateOnly? dateOnly)
    {
        if (dateOnly is null) return null;
        return new DateTime(dateOnly.Value.Year, dateOnly.Value.Month, dateOnly.Value.Day);
    }

    /// <summary>
    /// Converts a DateTime object to a DateOnly object.
    /// </summary>
    /// <param name="dateTime">The DateTime object to be converted.</param>
    /// <returns>The corresponding DateOnly object with the same date.</returns>
    public static DateOnly? ToDateOnly(this DateTime? dateTime)
    {
        if (dateTime is null) return null;
        return new DateOnly(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day);
    }

    public static DateOnly CalculateNextDueDate(this ERecursiveFrequency recursiveFrequency, DateOnly baseDate)
    {
        var dateOnly = recursiveFrequency switch
        {
            ERecursiveFrequency.Daily => baseDate.AddDays(1),
            ERecursiveFrequency.Weekly => baseDate.AddDays(7),
            ERecursiveFrequency.Monthly => baseDate.AddMonths(1),
            ERecursiveFrequency.Yearly => baseDate.AddYears(1),
            _ => throw new ArgumentOutOfRangeException()
        };
        return dateOnly;
    }
}