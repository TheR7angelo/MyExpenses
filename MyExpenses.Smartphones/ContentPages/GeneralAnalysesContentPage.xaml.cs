using MyExpenses.Models.Config.Interfaces;

namespace MyExpenses.Smartphones.ContentPages;

public partial class GeneralAnalysesContentPage
{
    public static readonly BindableProperty ButtonTextAccountAnalyzedByMonthProperty =
        BindableProperty.Create(nameof(ButtonTextAccountAnalyzedByMonth), typeof(string),
            typeof(GeneralAnalysesContentPage));

    public string ButtonTextAccountAnalyzedByMonth
    {
        get => (string)GetValue(ButtonTextAccountAnalyzedByMonthProperty);
        set => SetValue(ButtonTextAccountAnalyzedByMonthProperty, value);
    }

    public GeneralAnalysesContentPage()
    {
        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        ButtonTextAccountAnalyzedByMonth = "ButtonTextAccountAnalyzedByMonth";
    }

    private void ButtonAccountAnalyzedByMonth_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}