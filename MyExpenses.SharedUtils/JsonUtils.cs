using System.Reflection;
using Newtonsoft.Json;

namespace MyExpenses.SharedUtils;

public static class JsonUtils
{
    public static string ToJson(this object obj, Formatting formatting = Formatting.Indented)
        => JsonConvert.SerializeObject(obj, formatting);

    public static T? ToObject<T>(this string json)
    {
        if (File.Exists(json)) json = File.ReadAllText(json);

        return JsonConvert.DeserializeObject<T>(json);
    }

    public static T? ReadFromAssembly<T>(this Assembly assembly, string filename)
    {
        var resources = assembly.GetManifestResourceNames();
        var resourceName = resources.FirstOrDefault(s => s.Contains(filename));
        if (resourceName is null) return default;

        using var stream = assembly.GetManifestResourceStream(resourceName)!;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The StreamReader is necessary here to read the JSON file and convert it to a dictionary.
        // The JSON file contains the necessary information to authenticate the application with Dropbox.
        using var reader = new StreamReader(stream);

        var jsonStr = reader.ReadToEnd();

        return jsonStr.ToObject<T>();
    }
}