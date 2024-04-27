using System.Reflection;

namespace MyExpenses.Wpf.Utils;

public static class PropertyCopyHelper
{
    public static void CopyProperties<TSource, TDestination>(TSource source, TDestination destination)
        where TSource: class
        where TDestination: class
    {
        var sourceProperties = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach(var sourceProperty in sourceProperties)
        {
            var destinationProperty = destinationProperties.FirstOrDefault(d => d.Name == sourceProperty.Name && d.PropertyType == sourceProperty.PropertyType);

            if (destinationProperty == null || !destinationProperty.CanWrite) continue;
            var value = sourceProperty.GetValue(source);
            destinationProperty.SetValue(destination, value);
        }
    }
}