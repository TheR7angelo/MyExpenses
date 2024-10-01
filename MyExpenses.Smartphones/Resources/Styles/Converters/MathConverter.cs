using System.Globalization;

namespace MyExpenses.Smartphones.Resources.Styles.Converters;

public sealed class MathConverter : IValueConverter
{
    public double Offset { get; set; }
    public MathOperation Operation { get; set; }

    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        try
        {
            var value1 = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
            var value2 = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
            return Operation switch
            {
                MathOperation.Add => value1 + value2 + Offset,
                MathOperation.Divide => value1 / value2 + Offset,
                MathOperation.Multiply => value1 * value2 + Offset,
                MathOperation.Subtract => value1 - value2 + Offset,
                MathOperation.Pow => Math.Pow(value1, value2) + Offset,
                _ => Binding.DoNothing
            };
        }
        catch (FormatException)
        {
            return Binding.DoNothing;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}

public enum MathOperation
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Pow
}