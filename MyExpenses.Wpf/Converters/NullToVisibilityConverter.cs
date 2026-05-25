using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

/// <summary>
/// A value converter that converts null or empty values to a <see cref="Visibility"/> enumeration value.
/// Null or empty values are converted to <see cref="Visibility.Collapsed"/>, while non-null values
/// are converted to <see cref="Visibility.Visible"/>. The conversion logic is influenced by the
/// <see cref="MyExpenses.Presentation.Converters.NullToBoolConverter"/> class.
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Converts a value into a <see cref="System.Windows.Visibility"/> enumeration value.
    /// Null or empty values are converted to <see cref="System.Windows.Visibility.Collapsed"/>,
    /// while non-null values are converted to <see cref="System.Windows.Visibility.Visible"/>.
    /// </summary>
    /// <param name="value">The value to convert. Can be null or an object.</param>
    /// <param name="targetType">The target type of the conversion. This parameter is not used.</param>
    /// <param name="parameter">An optional parameter that can modify the conversion logic.</param>
    /// <param name="culture">The culture to use during conversion. This parameter is not used.</param>
    /// <returns>
    /// Returns <see cref="System.Windows.Visibility.Collapsed"/> for null or empty values.
    /// Returns <see cref="System.Windows.Visibility.Visible"/> otherwise.
    /// </returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Presentation.Converters.NullToBoolConverter.Convert(value, parameter)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <summary>
    /// Converts a <see cref="Visibility"/> enumeration value back to its original type.
    /// This method is not implemented and will throw a <see cref="NotImplementedException"/> when called.
    /// </summary>
    /// <param name="value">The <see cref="Visibility"/> value to convert back. This parameter is not used.</param>
    /// <param name="targetType">The target type of the conversion. This parameter is not used.</param>
    /// <param name="parameter">An optional parameter that can modify the conversion logic. This parameter is not used.</param>
    /// <param name="culture">The culture to use during conversion. This parameter is not used.</param>
    /// <returns>Throws a <see cref="NotImplementedException"/>.</returns>
    /// <exception cref="NotImplementedException">This method is not implemented.</exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}