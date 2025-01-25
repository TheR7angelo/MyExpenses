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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        return context.TRecursiveFrequencies
            .First(s => s.Id.Equals(id))
            .Description;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}