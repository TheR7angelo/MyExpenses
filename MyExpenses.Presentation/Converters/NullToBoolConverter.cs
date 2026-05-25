namespace MyExpenses.Presentation.Converters;

/// <summary>
/// Provides functionality to convert null or empty values to a boolean representation.
/// </summary>
public static class NullToBoolConverter
{
    /// <summary>
    /// Converts a value to a boolean representation based on its null or empty state.
    /// Allows inverse output based on the provided parameter.
    /// </summary>
    /// <param name="value">The value to evaluate for null or emptiness.</param>
    /// <param name="parameter">
    /// A parameter that can modify the conversion behavior. If the parameter is set to "Inverse" or "Not",
    /// the output will be inverted.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if the value is null or empty; otherwise, <c>false</c>.
    /// If the parameter specifies an inverse logic, the return values will be flipped.
    /// </returns>
    public static bool Convert(object? value, object? parameter)
    {
        var isNull = value is null || (value is string s && string.IsNullOrEmpty(s));

        var inverse = parameter is not null &&
                      (parameter.ToString()?.Equals("Inverse", StringComparison.OrdinalIgnoreCase) == true ||
                       parameter.ToString()?.Equals("Not", StringComparison.OrdinalIgnoreCase) == true);

        return inverse ? isNull : !isNull;
    }
}