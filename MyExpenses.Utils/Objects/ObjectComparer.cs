namespace MyExpenses.Utils.Objects;

public static class ObjectComparer
{
    /// <summary>
    /// Compares two objects to determine if they are equal by checking their properties and fields.
    /// </summary>
    /// <param name="obj1">The first object to compare.</param>
    /// <param name="obj2">The second object to compare.</param>
    /// <typeparam name="T">The type of the objects being compared.</typeparam>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public static bool AreEqual<T>(this T obj1, T obj2)
    {
        if (obj1 is null || obj2 is null) return obj1 is null && obj2 is null;

        var type = typeof(T);

        if (!ArePropertiesEqual(type, obj1, obj2)) return false;
        if (!AreFieldsEqual(type, obj1, obj2)) return false;

        return true;
    }

    /// <summary>
    /// Compares the properties of two objects to determine if they're equal.
    /// </summary>
    /// <param name="type">The type of the objects being compared.</param>
    /// <param name="obj1">The first object to compare.</param>
    /// <param name="obj2">The second object to compare.</param>
    /// <typeparam name="T">The type of objects being compared.</typeparam>
    /// <returns>True if all properties of the objects are equal; otherwise, false.</returns>
    private static bool ArePropertiesEqual<T>(Type type, T obj1, T obj2)
    {
        foreach (var property in type.GetProperties())
        {
            var value1 = property.GetValue(obj1);
            var value2 = property.GetValue(obj2);

            if (!ValuesAreEqual(value1, value2)) return false;
        }

        return true;
    }

    /// <summary>
    /// Compares the fields of two objects to determine if they're equal.
    /// </summary>
    /// <param name="type">The type of the objects being compared.</param>
    /// <param name="obj1">The first object to compare.</param>
    /// <param name="obj2">The second object to compare.</param>
    /// <typeparam name="T">The type of objects being compared.</typeparam>
    /// <return>True if all fields of the objects are equal; otherwise, false.</return>
    private static bool AreFieldsEqual<T>(Type type, T obj1, T obj2)
    {
        foreach (var field in type.GetFields())
        {
            var value1 = field.GetValue(obj1);
            var value2 = field.GetValue(obj2);

            if (!ValuesAreEqual(value1, value2)) return false;
        }

        return true;
    }

    /// <summary>
    /// Compares two values to determine if they are equal, handling special cases for collections and null values.
    /// </summary>
    /// <param name="value1">The first value to compare.</param>
    /// <param name="value2">The second value to compare.</param>
    /// <return>True if the values are equal; otherwise, false.</return>
    private static bool ValuesAreEqual(object? value1, object? value2)
    {
        if (value1 is null && value2 is null) return true;
        if (value1 is null || value2 is null) return false;

        if (value1 is IEnumerable<object> collection1 && value2 is IEnumerable<object> collection2)
        {
            return CollectionsAreEqual(collection1, collection2);
        }

        return value1.Equals(value2);
    }

    /// <summary>
    /// Compares two collections of objects for equality by checking if they contain the same elements in the same order.
    /// </summary>
    /// <param name="collection1">The first collection to compare.</param>
    /// <param name="collection2">The second collection to compare.</param>
    /// <return>True if the collections are equal; otherwise, false.</return>
    private static bool CollectionsAreEqual(IEnumerable<object> collection1, IEnumerable<object> collection2)
    {
        using var enumerator1 = collection1.GetEnumerator();
        using var enumerator2 = collection2.GetEnumerator();

        while (enumerator1.MoveNext())
        {
            if (!enumerator2.MoveNext() || !ValuesAreEqual(enumerator1.Current, enumerator2.Current))
            {
                return false;
            }
        }

        return !enumerator2.MoveNext();
    }
}