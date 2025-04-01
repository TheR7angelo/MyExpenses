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
    /// Calculates the next due date for a recurrent expense based on the given frequency, base date, payment mode, and cycle count.
    /// </summary>
    /// <param name="recursiveFrequency">The recurrence frequency (e.g., daily, weekly, monthly, etc.).</param>
    /// <param name="baseDate">The starting date for the recurrence calculation.</param>
    /// <param name="modePayment">The payment mode which may influence adjustments (e.g., handling weekends).</param>
    /// <param name="cycle">An optional parameter representing the number of recurrence cycles, defaulting to 1. Must be greater than or equal to 1.</param>
    /// <returns>The calculated next due date as a DateOnly object.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the cycle parameter is less than 1.</exception>
    public static DateOnly CalculateNextDueDate(this ERecursiveFrequency recursiveFrequency, DateOnly baseDate,
        EModePayment modePayment, int cycle = 1)
    {
        if (cycle < 1) throw new ArgumentOutOfRangeException(nameof(cycle), @"Cycle must be greater than or equal to 1.");

        var dateOnly = recursiveFrequency.CalculateNextDueDate(baseDate, cycle);
        if (modePayment is EModePayment.BankDirectDebit) dateOnly = dateOnly.AdjustForWeekends();
        return dateOnly;
    }

    /// <summary>
    /// Calculates the next due date based on a specified recursive frequency, starting from a given base date and cycle.
    /// </summary>
    /// <param name="recursiveFrequency">The frequency of recurrence which determines how the next due date is calculated.</param>
    /// <param name="baseDate">The starting date from which the recurrence calculation begins.</param>
    /// <param name="cycle">The recurrence cycle multiplier to calculate how many intervals to add to the base date.</param>
    /// <returns>The calculated next due date as a DateOnly object.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported recursive frequency is provided.</exception>
    private static DateOnly CalculateNextDueDate(this ERecursiveFrequency recursiveFrequency, DateOnly baseDate,
        int cycle)
    {
        var dateOnly = recursiveFrequency switch
        {
            ERecursiveFrequency.Daily => baseDate.AddDays(1 * cycle),
            ERecursiveFrequency.Weekly => baseDate.AddDays(7 * cycle),
            ERecursiveFrequency.Monthly => baseDate.AddMonths(1 * cycle),
            ERecursiveFrequency.Bimonthly => baseDate.AddMonths(2 * cycle),
            ERecursiveFrequency.Trimonthly => baseDate.AddMonths(3 * cycle),
            ERecursiveFrequency.Quarterly => baseDate.AddMonths(4 * cycle),
            ERecursiveFrequency.SixMonthly => baseDate.AddMonths(6 * cycle),
            ERecursiveFrequency.Yearly => baseDate.AddYears(1 * cycle),
            _ => baseDate
        };

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