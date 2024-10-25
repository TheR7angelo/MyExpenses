using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

/// <summary>
/// A value converter that converts numerical values to percentages and vice versa.
/// </summary>
public class PercentageConverter : IValueConverter
{
    /// <summary>
    /// Converts the given value to a percentage of the original value based on the parameter.
    /// </summary>
    /// <param name="value">The original value to be converted.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The percentage to apply as a multiplier to the original value, passed as a string.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The value multiplied by the percentage, or 0 if the conversion fails.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!double.TryParse(parameter?.ToString(), out var percentage)) return 0;
        if (value is not double number) return 0;
        if (number.Equals(-1)) return 0;

        percentage /= 100.0;
        return number * percentage;
    }

    /// <summary>
    /// Converts the given value back to its original form based on the parameter.
    /// </summary>
    /// <param name="value">The converted value to be reverted.</param>
    /// <param name="targetType">The target type of the original value.</param>
    /// <param name="parameter">The percentage used in the original conversion, passed as a string.</param>
    /// <param name="culture">The culture to use in the conversion back.</param>
    /// <returns>The original value before conversion, or throws a NotImplementedException if the conversion back is not implemented.</returns>
    /// <exception cref="NotImplementedException">Throws when the conversion back is not implemented.</exception>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}