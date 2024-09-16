using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.Wpf.Helps;

namespace MyExpenses.Wpf.UserControls.Helps;

public partial class HowToUseControl
{
    public static readonly DependencyProperty HowToUseCulturePathProperty =
        DependencyProperty.Register(nameof(HowToUseCulturePath), typeof(HowToUseCulturePath), typeof(HowToUseControl),
            new PropertyMetadata(default(HowToUseCulturePath)));

    public HowToUseCulturePath? HowToUseCulturePath
    {
        get => (HowToUseCulturePath)GetValue(HowToUseCulturePathProperty);
        set => SetValue(HowToUseCulturePathProperty, value);
    }

    public List<HowToUseCulturePath> HowToUseCulturePaths { get; }

    public HowToUseControl()
    {
        HowToUseCulturePaths = GetCultureInfoHowToUse();

        InitializeComponent();

        InitializeAsync();
    }

    #region Function

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

            var cultureInfo = allCulture.FirstOrDefault(c =>
                c.EnglishName.Contains(cultureName, StringComparison.CurrentCultureIgnoreCase));
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

    private async void InitializeAsync()
    {
        await WebView2.EnsureCoreWebView2Async();

        var currentCulture = CultureInfo.CurrentCulture;
        HowToUseCulturePath = HowToUseCulturePaths.First(s => s.CultureInfo.TwoLetterISOLanguageName.ToLower().Equals(currentCulture.TwoLetterISOLanguageName.ToLower()));

        if (HowToUseCulturePath is null) return;
        WebView2.CoreWebView2.Navigate(HowToUseCulturePath.Path);

        ListView.SelectionChanged += ListView_SelectionChanged;
    }

    #endregion

    #region Action

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count <= 0) return;
        if (e.AddedItems[0] is not HowToUseCulturePath selectedItem) return;

        WebView2.CoreWebView2.Navigate(selectedItem.Path);
    }

    #endregion
}