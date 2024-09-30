using Newtonsoft.Json;

namespace MyExpenses.Utils;

public static class JsonUtils
{
    public static string ToJson(this object obj, Formatting formatting = Formatting.Indented)
        => JsonConvert.SerializeObject(obj, formatting);

    public static T? ToObject<T>(this string json)
    {
        if (File.Exists(json)) json = File.ReadAllText(json);

        return JsonConvert.DeserializeObject<T>(json);
    }
}