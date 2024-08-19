using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Utils.Properties;

public static class PropertiesUtils
{
    public static PropertyInfo? GetPropertiesInfoByName<T, TAttribute>(this string name) where TAttribute : Attribute
    {
        var type = typeof(T);
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

    public static object? GetPropertiesInfoByName<TAttribute>(this string name, object element) where TAttribute : Attribute
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

    public static object? GetValueByProperty<TAttribute>(this PropertyInfo property) where TAttribute : Attribute
    {
        var attribute = property.GetCustomAttribute<TAttribute>();

        return attribute switch
        {
            ColumnAttribute columnAttribute => columnAttribute.Name,
            MaxLengthAttribute maxLengthAttribute => maxLengthAttribute.Length,
            PrecisionAttribute precisionAttribute => precisionAttribute.Precision,
            _ => null
        };
    }
}