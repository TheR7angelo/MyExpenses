namespace MyExpenses.Utils.Objects;

public static class ObjectComparer
{
    public static bool AreEqual<T>(this T obj1, T obj2)
    {
        if (obj1 is null || obj2 is null) return obj1 is null && obj2 is null;

        var type = typeof(T);
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var value1 = property.GetValue(obj1);
            var value2 = property.GetValue(obj2);

            if (value1 is null && value2 is null) continue;
            if (value1 is null || value2 is null) return false;
            if (!value1.Equals(value2)) return false;
        }

        var fields = type.GetFields();
        foreach (var field in fields)
        {
            var value1 = field.GetValue(obj1);
            var value2 = field.GetValue(obj2);

            if (value1 is null && value2 is null) continue;
            if (value1 is null || value2 is null) return false;
            if (!value1.Equals(value2)) return false;
        }
        return true;
    }
}