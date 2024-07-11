using MyExpenses.Models.Config;
using Newtonsoft.Json;

namespace MyExpenses.Utils;

public static class Config
{
    private static string ConfigurationFilePath { get; } = Path.GetFullPath("appsettings.json");
    
    public static Configuration Configuration { get; private set; }

    static Config()
    {
        if (File.Exists(ConfigurationFilePath))
        {
            Configuration = ReadConfiguration();
        }
        else
        {
            var configuration = new Configuration();
            configuration.WriteConfiguration();

            Configuration = configuration;
        }
    }

    private static Configuration ReadConfiguration()
    {
        var json = File.ReadAllText(ConfigurationFilePath);
        var configuration = JsonConvert.DeserializeObject<Configuration>(json) ?? new Configuration();
        return configuration;
    }

    public static void WriteConfiguration(this Configuration configuration)
    {
        var json = JsonConvert.SerializeObject(configuration, Formatting.Indented);
        File.WriteAllText(ConfigurationFilePath, json);
    }
}