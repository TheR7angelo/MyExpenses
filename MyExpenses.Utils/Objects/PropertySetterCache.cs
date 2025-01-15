using System.Linq.Expressions;
using System.Reflection;

namespace MyExpenses.Utils.Objects;

/// <summary>
/// Provides a cache mechanism for dynamically creating and storing property setter actions
/// for a specified type.
/// </summary>
/// <typeparam name="T">The type of object for which the property setters are created.</typeparam>
public static class PropertySetterCache<T>
{
    private static readonly Dictionary<string, Action<T, object?>> SettersCache = new();

    /// <summary>
    /// Creates a property setter action for the given property of the type <typeparamref name="T"/>.
    /// The setter can be used to dynamically set the value of a specified property on an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="property">The <see cref="PropertyInfo"/> of the property for which the setter is to be created.</param>
    /// <returns>An <see cref="Action{T, Object}"/> that sets the property value for a specific instance of type <typeparamref name="T"/>.</returns>
    public static Action<T, object?> CreateSetter(PropertyInfo property)
    {
        if (SettersCache.TryGetValue(property.Name, out var cachedSetter))
            return cachedSetter;

        var instanceParam = Expression.Parameter(typeof(T), "instance");
        var valueParam = Expression.Parameter(typeof(object), "value");

        var castValue = Expression.Convert(valueParam, property.PropertyType); // Conversion vers le type attendu
        var propertyExpr = Expression.Property(instanceParam, property);
        var assignExpr = Expression.Assign(propertyExpr, castValue);

        var setter = Expression.Lambda<Action<T, object?>>(assignExpr, instanceParam, valueParam).Compile();
        SettersCache[property.Name] = setter;

        return setter;
    }
}