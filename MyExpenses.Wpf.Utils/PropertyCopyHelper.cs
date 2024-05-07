using System.Reflection;

namespace MyExpenses.Wpf.Utils;

/// <summary>
/// A static class that provides methods for copying properties from one object to another.
/// </summary>
public static class PropertyCopyHelper
{
    /// <summary>
    /// Copies the properties from one object to another.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object.</typeparam>
    /// <typeparam name="TDestination">The type of the destination object.</typeparam>
    /// <param name="source">The source object.</param>
    /// <param name="destination">The destination object.</param>
    public static void CopyPropertiesTo<TSource, TDestination>(this TSource source, TDestination destination)
        where TSource : class
        where TDestination : class
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