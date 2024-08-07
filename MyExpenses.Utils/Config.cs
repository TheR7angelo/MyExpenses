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

    /// <summary>
    /// Reads the configuration from the appsettings.json file.
    /// </summary>
    /// <returns>The Configuration object representing the configuration settings.</returns>
    private static Configuration ReadConfiguration()
    {
        var json = File.ReadAllText(ConfigurationFilePath);
        var configuration = JsonConvert.DeserializeObject<Configuration>(json) ?? new Configuration();
        return configuration;
    }

    /// <summary>
    /// Writes the configuration object to the appsettings.json file.
    /// </summary>
    /// <param name="configuration">The Configuration object representing the configuration settings.</param>
    public static void WriteConfiguration(this Configuration configuration)
    {
        var json = JsonConvert.SerializeObject(configuration, Formatting.Indented);
        File.WriteAllText(ConfigurationFilePath, json);
    }
}