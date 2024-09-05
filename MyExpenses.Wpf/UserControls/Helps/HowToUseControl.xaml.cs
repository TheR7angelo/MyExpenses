using System.Globalization;
using System.IO;
using MyExpenses.Models.Wpf.Helps;

namespace MyExpenses.Wpf.UserControls.Helps;

public partial class HowToUseControl
{
    public List<HowToUseCulturePath> HowToUseCulturePaths { get; }

    public HowToUseControl()
    {
        HowToUseCulturePaths = GetCultureInfoHowToUse();

        InitializeComponent();
    }

    private static List<HowToUseCulturePath> GetCultureInfoHowToUse()
    {
        var directory = Path.GetFullPath("Resources");
        directory = Path.Join(directory, "How to use");

        var results = new List<HowToUseCulturePath>();
        var allCulture = CultureInfo.GetCultures(CultureTypes.AllCultures);
        var files = Directory.GetFiles(directory, "*.pdf");
        foreach (var file in files)
        {
            var filename = Path.GetFileNameWithoutExtension(file);
            var filenameSplit = filename.Split('_');
            if (filenameSplit.Length < 1) continue;

            var cultureName = filenameSplit[1];

            var cultureInfo = allCulture.FirstOrDefault(c => c.EnglishName.Contains(cultureName, StringComparison.CurrentCultureIgnoreCase));
            if (cultureInfo is null) continue;

            var howToUseCulturePath = new HowToUseCulturePath
            {
                CultureInfo = cultureInfo,
                Path = file
            };
            results.Add(howToUseCulturePath);
        }

        results = results.OrderBy(r => r.CultureInfo.Name).ToList();

        return results;
    }
}