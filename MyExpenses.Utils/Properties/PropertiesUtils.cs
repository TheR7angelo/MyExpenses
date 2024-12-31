using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MyExpenses.Utils.Properties;

public static class PropertiesUtils
{
    public static PropertyInfo? GetPropertiesInfoByName<TAttribute>(this string name, Type type) where TAttribute : Attribute
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
}