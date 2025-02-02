using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MyExpenses.SharedUtils.Properties;

/// <summary>
/// A utility class for working with object properties and retrieving property information
/// based on attribute types and property names.
/// </summary>
public static class PropertiesUtils
{
    /// <summary>
    /// Retrieves the property information from the specified type based on a given name
    /// and the presence of a specific attribute type.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute used to identify the property.</typeparam>
    /// <param name="name">The name of the property to find, often matching the attribute name.</param>
    /// <param name="type">The type containing the properties to search within.</param>
    /// <returns>
    /// A PropertyInfo object that matches the specified name and attribute, or null if no match is found.
    /// </returns>
    public static PropertyInfo? GetPropertiesInfoByName<TAttribute>(this string name, Type type)
        where TAttribute : Attribute
    {
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<TAttribute>();

            switch (attribute)
            {
                case null:
                    continue;
                case ColumnAttribute columnAttribute when columnAttribute.Name == name:
                    return property;
            }
        }

        return null;
    }

    /// <summary>
    /// Retrieves the property information from an array of PropertyInfo based on a given name
    /// and the presence of a specific attribute type.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute used to identify the property.</typeparam>
    /// <param name="name">The name of the property to find, often matching the attribute name.</param>
    /// <param name="propertyInfos">An array of PropertyInfo objects to search within.</param>
    /// <returns>
    /// A PropertyInfo object that matches the specified name and attribute, or null if no match is found.
    /// </returns>
    public static PropertyInfo? GetPropertiesInfoByName<TAttribute>(this string name, PropertyInfo[] propertyInfos)
        where TAttribute : Attribute
    {
        foreach (var property in propertyInfos)
        {
            var attribute = property.GetCustomAttribute<TAttribute>();

            switch (attribute)
            {
                case null:
                    continue;
                case ColumnAttribute columnAttribute when columnAttribute.Name == name:
                    return property;
            }
        }

        return null;
    }

    /// <summary>
    /// Retrieves the information of a property annotated with a specific attribute type from an object,
    /// based on the given property name.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute used to identify the property.</typeparam>
    /// <param name="name">The name of the property to locate, typically corresponding to the attribute's name.</param>
    /// <param name="element">The object instance containing the properties to search through.</param>
    /// <returns>
    /// The value of the property that matches the specified name and attribute, or null if no such property is found.
    /// </returns>
    public static object? GetPropertiesInfoByName<TAttribute>(this string name, object element)
        where TAttribute : Attribute
    {
        var type = element.GetType();
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<TAttribute>();

            switch (attribute)
            {
                case null:
                    continue;
                case ColumnAttribute columnAttribute when columnAttribute.Name == name:
                    return property.GetValue(element);
            }
        }

        return null;
    }

    /// <summary>
    /// Retrieves the property information from a set of destination properties
    /// based on a specified property name and source type.
    /// </summary>
    /// <param name="destinationProperties">An array of PropertyInfo objects representing the destination properties to search within.</param>
    /// <param name="name">The name of the property to search for.</param>
    /// <param name="sourceType">The type of the source property to match against.</param>
    /// <returns>
    /// A PropertyInfo object that matches the specified name and type, or null if no match is found.
    /// </returns>
    private static PropertyInfo? GetPropertiesInfoByNameAndType(this PropertyInfo[] destinationProperties, string name,
        Type sourceType)
    {
        foreach (var propertyInfo in destinationProperties)
        {
            if (propertyInfo.Name.Equals(name) && (Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType) == sourceType)
            {
                return propertyInfo;
            }
        }

        return null;
    }

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

        foreach (var sourceProperty in sourceProperties)
        {
            var sourceType = Nullable.GetUnderlyingType(sourceProperty.PropertyType) ?? sourceProperty.PropertyType;

            var destinationProperty = destinationProperties.GetPropertiesInfoByNameAndType(sourceProperty.Name, sourceType);

            if (destinationProperty == null || !destinationProperty.CanWrite) continue;
            var value = sourceProperty.GetValue(source);
            destinationProperty.SetValue(destination, value);
        }
    }
}