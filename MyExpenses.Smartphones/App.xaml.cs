using System.Reflection;
using MyExpenses.Models.IO.Smartphones;
using MyExpenses.Utils;

namespace MyExpenses.Smartphones;

public partial class App
{
    public App()
    {
        SetInitialFile();

        InitializeComponent();

        MainPage = new AppShell();
    }

    private void SetInitialFile()
    {
        var jsonFile = Path.Join(FileSystem.AppDataDirectory, "AppVersionInfo.json");

        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        var needUpdateFiles = false;
        if (File.Exists(jsonFile))
        {
            var appVersionInfo = jsonFile.ToObject<AppVersionInfo>()!;
            if (currentVersion > appVersionInfo.Version)
            {
                needUpdateFiles = true;
            }
        }
        else
        {
            var appVersionInfo = new AppVersionInfo
            {
                Version = currentVersion,
                LastUpdated = DateTime.Now
            };
            var json = appVersionInfo.ToJson();
            File.WriteAllText(jsonFile, json);

            needUpdateFiles = true;
        }

        if (!needUpdateFiles) return;

        var fileManager = new FileManager();
        fileManager.AddAllFiles();
    }
}