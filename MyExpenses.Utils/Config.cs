using MyExpenses.Models.Config;
using MyExpenses.SharedUtils.GlobalInfos;

namespace MyExpenses.Utils;

public static class Config
{
    public static Configuration Configuration { get; private set; }

    static Config()
    {
        if (File.Exists(OsInfos.ConfigurationFilePath))
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
        var json = File.ReadAllText(OsInfos.ConfigurationFilePath);
        var configuration = json.ToObject<Configuration>() ?? new Configuration();
        return configuration;
    }

    /// <summary>
    /// Writes the configuration object to the appsettings.json file.
    /// </summary>
    /// <param name="configuration">The Configuration object representing the configuration settings.</param>
    public static void WriteConfiguration(this Configuration configuration)
    {
        var json = configuration.ToJson();
        File.WriteAllText(OsInfos.ConfigurationFilePath, json);
    }
}