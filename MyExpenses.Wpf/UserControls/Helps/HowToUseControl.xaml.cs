using System.Globalization;
using System.IO;

namespace MyExpenses.Wpf.UserControls.Helps;

public partial class HowToUseControl
{
    public List<CultureInfo> CultureInfos { get; }
    public HowToUseControl()
    {
        CultureInfos = GetCultureInfoHowToUse();

        InitializeComponent();
    }

    private static List<CultureInfo> GetCultureInfoHowToUse()
    {
        var directory = Path.GetFullPath("Resources");
        directory = Path.Join(directory, "How to use");

        var results = new List<CultureInfo>();
        var allCulture = CultureInfo.GetCultures(CultureTypes.AllCultures);
        var files = Directory.GetFiles(directory, "*.pdf");
        foreach (var file in files)
        {
            var filename = Path.GetFileNameWithoutExtension(file);
            var filenameSplit = filename.Split('_');
            if (filenameSplit.Length < 1) continue;

            var cultureName = filenameSplit[1];

            var cultureInfo = allCulture.FirstOrDefault(c => c.EnglishName.Contains(cultureName, StringComparison.CurrentCultureIgnoreCase));
            if (cultureInfo is not null) results.Add(cultureInfo);
        }

        return results;
    }
}