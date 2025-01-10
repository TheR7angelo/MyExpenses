namespace MyExpenses.Models.IO.Sig.Shp.Converters;

public static class ConversionExtensions
{
    public static object? ConvertTo(this object? value, Type targetType)
    {
        if (value == null)
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

        if (targetType.IsInstanceOfType(value))
        {
            return value;
        }

        if ((targetType == typeof(bool) || targetType == typeof(bool?) ) &&
            value is string or int or double)
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return value.ToBool();
        }

        try
        {
            return Convert.ChangeType(value, targetType);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}