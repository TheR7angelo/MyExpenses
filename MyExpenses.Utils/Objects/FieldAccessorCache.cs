using System.Linq.Expressions;
using System.Reflection;

namespace MyExpenses.Utils.Objects;

/// <summary>
/// Provides a cache mechanism for dynamically creating and storing field getter and setter actions
/// for a specified type.
/// </summary>
/// <typeparam name="T">The type of object for which the field getters and setters are created.</typeparam>
public static class FieldAccessorCache<T>
{
    // ReSharper disable HeapView.ObjectAllocation.Evident
    // Static dictionaries to cache field getter and setter delegates,
    // optimizing reflection for improved performance and reuse.
    private static readonly Dictionary<string, Func<T, object?>> FieldGettersCache = new();
    private static readonly Dictionary<string, Action<T, object?>> FieldSettersCache = new();
    // ReSharper restore HeapView.ObjectAllocation.Evident

    /// <summary>
    /// Creates a field getter function for the given field of the type <typeparamref name="T"/>.
    /// The getter can be used to dynamically retrieve the value of a specified field on an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> of the field for which the getter is to be created.</param>
    /// <returns>A <see cref="Func{T, Object}"/> that gets the field value for a specific instance of type <typeparamref name="T"/>.</returns>
    public static Func<T, object?> CreateGetter(FieldInfo field)
    {
        if (FieldGettersCache.TryGetValue(field.Name, out var cachedGetter))
            return cachedGetter;

        var instanceParam = Expression.Parameter(typeof(T), "instance");
        var fieldExpr = Expression.Field(instanceParam, field);
        var convertExpr = Expression.Convert(fieldExpr, typeof(object)); // Conversion vers object pour les types de valeur

        var getter = Expression.Lambda<Func<T, object?>>(convertExpr, instanceParam).Compile();
        FieldGettersCache[field.Name] = getter;

        return getter;
    }

    /// <summary>
    /// Creates a field setter action for the given field of the type <typeparamref name="T"/>.
    /// The setter can be used to dynamically set the value of a specified field on an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> of the field for which the setter is to be created.</param>
    /// <returns>An <see cref="Action{T, Object}"/> that sets the field value for a specific instance of type <typeparamref name="T"/>.</returns>
    public static Action<T, object?> CreateSetter(FieldInfo field)
    {
        if (FieldSettersCache.TryGetValue(field.Name, out var cachedSetter))
            return cachedSetter;

        var instanceParam = Expression.Parameter(typeof(T), "instance");
        var valueParam = Expression.Parameter(typeof(object), "value");

        var castValue = Expression.Convert(valueParam, field.FieldType); // Conversion vers le type attendu
        var fieldExpr = Expression.Field(instanceParam, field);
        var assignExpr = Expression.Assign(fieldExpr, castValue);

        var setter = Expression.Lambda<Action<T, object?>>(assignExpr, instanceParam, valueParam).Compile();
        FieldSettersCache[field.Name] = setter;

        return setter;
    }
}