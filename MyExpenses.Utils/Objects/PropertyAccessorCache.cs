using System.Linq.Expressions;
using System.Reflection;

namespace MyExpenses.Utils.Objects;

/// <summary>
/// Provides a cache mechanism for dynamically creating and storing property setter actions
/// for a specified type.
/// </summary>
/// <typeparam name="T">The type of object for which the property setters are created.</typeparam>
public static class PropertyAccessorCache<T>
{
    // ReSharper disable HeapView.ObjectAllocation.Evident
    // Static dictionaries to cache property getter and setter delegates,
    // enhancing performance by reducing repetitive reflection operations.
    private static readonly Dictionary<string, Func<T, object?>> GettersCache = new();
    private static readonly Dictionary<string, Action<T, object?>> SettersCache = new();
    // ReSharper restore HeapView.ObjectAllocation.Evident

    /// <summary>
    /// Creates a property getter function for the given property of the type <typeparamref name="T"/>.
    /// The getter can be used to dynamically retrieve the value of a specified property on an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="property">The <see cref="PropertyInfo"/> of the property for which the getter is to be created.</param>
    /// <returns>A <see cref="Func{T, Object}"/> that gets the property value for a specific instance of type <typeparamref name="T"/>.</returns>
    public static Func<T, object?> CreateGetter(PropertyInfo property)
    {
        if (GettersCache.TryGetValue(property.Name, out var cachedGetter))
            return cachedGetter;

        var instanceParam = Expression.Parameter(typeof(T), "instance");
        var propertyExpr = Expression.Property(instanceParam, property);
        var convertExpr = Expression.Convert(propertyExpr, typeof(object)); // Conversion vers object pour les types de valeur

        var getter = Expression.Lambda<Func<T, object?>>(convertExpr, instanceParam).Compile();
        GettersCache[property.Name] = getter;

        return getter;
    }

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