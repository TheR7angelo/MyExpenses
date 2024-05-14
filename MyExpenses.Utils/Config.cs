using MyExpenses.Models.Config;
using Newtonsoft.Json;

namespace MyExpenses.Utils;

public static class Config
{
    private static string ConfigurationFilePath { get; } = Path.GetFullPath("appsettings.json");
    
    public static Configuration Configuration { get; }
    
    static Config()
    {
        if (File.Exists(ConfigurationFilePath))
        {
            var json = File.ReadAllText(ConfigurationFilePath);
            Configuration = JsonConvert.DeserializeObject<Configuration>(json) ?? new Configuration();
        }
        else
        {
            Configuration = new Configuration();
            var json = JsonConvert.SerializeObject(Configuration, Formatting.Indented);
            File.WriteAllText(ConfigurationFilePath, json);
        }
    }
}