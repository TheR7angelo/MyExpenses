using MyExpenses.Models.Sql.Bases.Enums;

namespace MyExpenses.Utils.DateTimes;

public static class DateTimeExtensions
{
    /// <summary>
    /// Converts a nullable DateTime object to a TimeSpan representing the time of day.
    /// </summary>
    /// <param name="dateTime">The nullable DateTime object to be converted.</param>
    /// <returns>A TimeSpan representing the time of day, or a default TimeSpan if the DateTime object is null.</returns>
    public static TimeSpan ToTimeSpan(this DateTime? dateTime)
    {
        var timeSpan = dateTime?.TimeOfDay ?? TimeSpan.Zero;
        return timeSpan;
    }

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

    /// <summary>
    /// Calculates the next due date for a given base date based on a specified recursive frequency and payment mode.
    /// </summary>
    /// <param name="recursiveFrequency">The frequency of recurrence, determining the interval for the next due date.</param>
    /// <param name="baseDate">The starting date from which the next due date will be calculated.</param>
    /// <param name="modePayment">The mode of payment, used to potentially influence the calculation.</param>
    /// <returns>The calculated next due date as a DateOnly object.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported recursive frequency is provided.</exception>
    public static DateOnly CalculateNextDueDate(this ERecursiveFrequency recursiveFrequency, DateOnly baseDate,
        EModePayment modePayment)
    {
        var dateOnly = recursiveFrequency switch
        {
            ERecursiveFrequency.Daily => baseDate.AddDays(1),
            ERecursiveFrequency.Weekly => baseDate.AddDays(7),
            ERecursiveFrequency.Monthly => baseDate.AddMonths(1),
            ERecursiveFrequency.Bimonthly => baseDate.AddMonths(2),
            ERecursiveFrequency.Trimonthly => baseDate.AddMonths(3),
            ERecursiveFrequency.Quarterly => baseDate.AddMonths(4),
            ERecursiveFrequency.SixMonthly => baseDate.AddMonths(6),
            ERecursiveFrequency.Yearly => baseDate.AddYears(1),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (modePayment is EModePayment.BankDirectDebit) dateOnly = dateOnly.AdjustForWeekends();
        return dateOnly;
    }

    /// <summary>
    /// Adjusts the given date to a weekday if it falls on a weekend.
    /// </summary>
    /// <param name="date">The date to be adjusted.</param>
    /// <returns>The adjusted date that falls on a weekday.</returns>
    public static DateOnly AdjustForWeekends(this DateOnly date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Saturday => date.AddDays(2), // Décaler au lundi
            DayOfWeek.Sunday => date.AddDays(1),  // Décaler au lundi
            _ => date
        };
    }
}