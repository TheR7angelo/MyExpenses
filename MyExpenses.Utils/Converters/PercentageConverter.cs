namespace MyExpenses.Utils.Converters;

public static class PercentageConverter
{
    /// <summary>
    /// Converts the provided value to a percentage of itself based on the parameter.
    /// </summary>
    /// <param name="value">The input value to be converted, expected to be of type double.</param>
    /// <param name="parameter">The percentage to apply, expressed as a string or another object parsable to a valid double.</param>
    /// <returns>The value after being multiplied by the specified percentage.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the parameter is not a valid percentage value or the value is not of type double.
    /// </exception>
    public static double Convert(object? value, object? parameter)
    {
        if (!double.TryParse(parameter?.ToString(), out var percentage))
            throw new ArgumentException(@"Parameter must be a valid percentage value", nameof(parameter));

        if (value is not double number)
            throw new ArgumentException(@"Value must be a double", nameof(value));

        percentage /= 100.0;
        return number * percentage;
    }

    /// <summary>
    /// Converts the provided value back to its original form using the specified parameter as the percentage factor.
    /// </summary>
    /// <param name="value">The value to be converted back, expected to be of type double.</param>
    /// <param name="parameter">The percentage factor, provided as a string or another object parsable to a valid double.</param>
    /// <returns>The original value before the percentage conversion was applied.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the parameter is not a valid percentage value or the value is not of type double.
    /// </exception>
    public static double ConvertBack(object? value, object? parameter)
    {
        if (!double.TryParse(parameter?.ToString(), out var percentage) || value is not double number)
            throw new ArgumentException(@"Invalid value or parameter");

        percentage /= 100.0;
        return number / percentage;
    }
}