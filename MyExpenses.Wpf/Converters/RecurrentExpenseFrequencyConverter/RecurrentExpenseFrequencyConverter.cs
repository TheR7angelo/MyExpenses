using System.Globalization;
using System.Windows.Data;
using MyExpenses.Models.Sql.Bases.Enums;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Converters.RecurrentExpenseFrequencyConverter;

public class RecurrentExpenseFrequencyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ERecursiveFrequency recursiveFrequency) return Binding.DoNothing;

        var id = (int)recursiveFrequency;

        using var context = new DataBaseContext();
        return context.TRecursiveFrequencies
            .First(s => s.Id.Equals(id))
            .Description;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}