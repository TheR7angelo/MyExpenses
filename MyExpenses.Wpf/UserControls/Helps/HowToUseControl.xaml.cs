using System.Globalization;
using System.IO;
using System.Windows.Controls;
using MyExpenses.Models.Wpf.Helps;

namespace MyExpenses.Wpf.UserControls.Helps;

public partial class HowToUseControl
{
    public List<HowToUseCulturePath> HowToUseCulturePaths { get; }

    public HowToUseControl()
    {
        HowToUseCulturePaths = GetCultureInfoHowToUse();

        InitializeComponent();

        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await WebView2.EnsureCoreWebView2Async();

        var url = HowToUseCulturePaths.First().Path;
        WebView2.CoreWebView2.Navigate(url);
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

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count <= 0) return;
        if (e.AddedItems[0] is not HowToUseCulturePath selectedItem) return;

        WebView2.CoreWebView2.Navigate(selectedItem.Path);
    }
}