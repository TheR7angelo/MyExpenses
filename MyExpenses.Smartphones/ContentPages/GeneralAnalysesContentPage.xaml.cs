using MyExpenses.Models.Config.Interfaces;
using MyExpenses.SharedUtils.Resources.Resx.AnalyticsManagement;
using MyExpenses.Smartphones.ContentPages.Analytics;
using MyExpenses.Smartphones.ContentPages.Analytics.AccountsCategorySumPositiveNegativeContent;
using MyExpenses.Smartphones.ContentPages.Analytics.AccountTotalEllipseContent;

namespace MyExpenses.Smartphones.ContentPages;

public partial class GeneralAnalysesContentPage
{
    public static readonly BindableProperty ButtonTextAccountsCategorySumPositiveNegativeControlProperty =
        BindableProperty.Create(nameof(ButtonTextAccountsCategorySumPositiveNegativeControl), typeof(string),
            typeof(GeneralAnalysesContentPage));

    public string ButtonTextAccountsCategorySumPositiveNegativeControl
    {
        get => (string)GetValue(ButtonTextAccountsCategorySumPositiveNegativeControlProperty);
        set => SetValue(ButtonTextAccountsCategorySumPositiveNegativeControlProperty, value);
    }

    public static readonly BindableProperty ButtonTextAccountTotalEllipseProperty =
        BindableProperty.Create(nameof(ButtonTextAccountTotalEllipse), typeof(string),
            typeof(GeneralAnalysesContentPage));

    public string ButtonTextAccountTotalEllipse
    {
        get => (string)GetValue(ButtonTextAccountTotalEllipseProperty);
        set => SetValue(ButtonTextAccountTotalEllipseProperty, value);
    }

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
        ButtonTextAccountAnalyzedByMonth = AnalyticsManagementResources.ButtonTextAnalyseByCategoryAndMonth;
        ButtonTextAccountTotalEllipse = AnalyticsManagementResources.TabItemAccountTotalEllipseControlHeader;
        ButtonTextAccountsCategorySumPositiveNegativeControl = AnalyticsManagementResources.TabItemAccountsCategorySumPositiveNegativeControl;
        // ButtonTextCumulativeSumChartControlHeader = AnalyticsManagementResources.TabItemCumulativeSumChartControlHeader;
        // ButtonTextAccountsCategorySumControlHeader = AnalyticsManagementResources.TabItemAccountsCategorySumControlHeader;
        // ButtonTextAccountsModePaymentMonthlySumControlHeader = AnalyticsManagementResources.TabItemAccountsModePaymentMonthlySumControlHeader;
        // ButtonTextBudgetsControlHeader = AnalyticsManagementResources.TabItemBudgetsControlHeader;
    }

    private void ButtonAccountAnalyzedByMonth_OnClicked(object? sender, EventArgs e)
        => _ = typeof(AccountAnalyzedByMonthContentPage).NavigateToAsync();

    private void ButtonAccountTotalEllipse_OnClicked(object? sender, EventArgs e)
        => _ = typeof(AccountTotalEllipseContentPage).NavigateToAsync();

    private void ButtonAccountsCategorySumPositiveNegative_OnClicked(object? sender, EventArgs e)
        => _ = typeof(AccountsCategorySumPositiveNegativeContentPage).NavigateToAsync();
}