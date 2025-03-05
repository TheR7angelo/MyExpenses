using System.Globalization;
using MyExpenses.Models.Sql;
using MyExpenses.Utils;

namespace MyExpenses.Smartphones.Converters;

public class ISqlConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // ReSharper disable once HeapView.ClosureAllocation
        if (value is not int id) return null;
        if (parameter is not Binding binding) return null;

        parameter = binding.Source.GetPropertyValue(binding.Path);
        if (parameter is not IEnumerable<ISql> enumerable) return null;

        // ReSharper disable once HeapView.DelegateAllocation
        return enumerable.FirstOrDefault(s => s.Id.Equals(id));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ISql iSql) return null;
        // ReSharper disable once HeapView.BoxingAllocation
        return iSql.Id;
    }
}
