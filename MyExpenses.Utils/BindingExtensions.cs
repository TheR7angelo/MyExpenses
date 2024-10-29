namespace MyExpenses.Utils;

public static class BindingExtensions
{
    public static object? GetPropertyValue(this object? src, string propertyName)
    {
        if (!propertyName.Contains('.')) return src?.GetType().GetProperty(propertyName)?.GetValue(src, null);

        var splitIndex = propertyName.IndexOf('.');
        var parent = propertyName[..splitIndex];
        var child = propertyName[(splitIndex + 1)..];
        var obj = src?.GetType().GetProperty(parent)?.GetValue(src, null);

        return GetPropertyValue(obj, child);
    }
}