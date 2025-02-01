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
}