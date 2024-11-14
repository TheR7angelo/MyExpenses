namespace MyExpenses.Utils.Objects;

public static class ObjectComparer
{
    public static bool AreEqual<T>(this T obj1, T obj2)
    {
        if (obj1 is null || obj2 is null) return obj1 is null && obj2 is null;

        var type = typeof(T);

        if (!ArePropertiesEqual(type, obj1, obj2)) return false;
        if (!AreFieldsEqual(type, obj1, obj2)) return false;

        return true;
    }

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