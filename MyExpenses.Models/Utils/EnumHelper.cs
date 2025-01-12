using System.Runtime.CompilerServices;

namespace MyExpenses.Models.Utils;

public static class EnumHelper<T> where T : struct, Enum
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly string[] Names;

    static EnumHelper()
    {
        var enumValues = Enum.GetValues<T>();
        Names = new string[enumValues.Length];

        for (var i = 0; i < enumValues.Length; i++)
        {
            Names[i] = Enum.GetName(typeof(T), enumValues.GetValue(i)!)!;
        }
    }

    public static string ToEnumString(T value)
    {
        var index = Unsafe.As<T, int>(ref value);
        return Names[index];
    }
}