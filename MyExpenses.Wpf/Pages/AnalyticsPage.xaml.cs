using System.Windows;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Wpf.Resources.Resx.Pages.AnalyticsPage;

namespace MyExpenses.Wpf.Pages;

public partial class AnalyticsPage
{
    public static readonly DependencyProperty TabItemAccountsModePaymentMonthlySumControlHeaderProperty =
        DependencyProperty.Register(nameof(TabItemAccountsModePaymentMonthlySumControlHeader), typeof(string),
            typeof(AnalyticsPage), new PropertyMetadata(default(string)));

    public string TabItemAccountsModePaymentMonthlySumControlHeader
    {
        get => (string)GetValue(TabItemAccountsModePaymentMonthlySumControlHeaderProperty);
        set => SetValue(TabItemAccountsModePaymentMonthlySumControlHeaderProperty, value);
    }

    public static readonly DependencyProperty TabItemCumulativeSumChartControlHeaderProperty =
        DependencyProperty.Register(nameof(TabItemCumulativeSumChartControlHeader), typeof(string),
            typeof(AnalyticsPage), new PropertyMetadata(default(string)));

    public string TabItemCumulativeSumChartControlHeader
    {
        get => (string)GetValue(TabItemCumulativeSumChartControlHeaderProperty);
        set => SetValue(TabItemCumulativeSumChartControlHeaderProperty, value);
    }

    public static readonly DependencyProperty TabItemAccountTotalEllipseControlHeaderProperty =
        DependencyProperty.Register(nameof(TabItemAccountTotalEllipseControlHeader), typeof(string),
            typeof(AnalyticsPage), new PropertyMetadata(default(string)));

    public string TabItemAccountTotalEllipseControlHeader
    {
        get => (string)GetValue(TabItemAccountTotalEllipseControlHeaderProperty);
        set => SetValue(TabItemAccountTotalEllipseControlHeaderProperty, value);
    }

    public static readonly DependencyProperty TabItemAccountsCategorySumControlHeaderProperty =
        DependencyProperty.Register(nameof(TabItemAccountsCategorySumControlHeader), typeof(string),
            typeof(AnalyticsPage), new PropertyMetadata(default(string)));

    public string TabItemAccountsCategorySumControlHeader
    {
        get => (string)GetValue(TabItemAccountsCategorySumControlHeaderProperty);
        set => SetValue(TabItemAccountsCategorySumControlHeaderProperty, value);
    }

    public static readonly DependencyProperty TabItemAccountsCategorySumPositiveNegativeControlProperty =
        DependencyProperty.Register(nameof(TabItemAccountsCategorySumPositiveNegativeControl), typeof(string),
            typeof(AnalyticsPage), new PropertyMetadata(default(string)));

    public string TabItemAccountsCategorySumPositiveNegativeControl
    {
        get => (string)GetValue(TabItemAccountsCategorySumPositiveNegativeControlProperty);
        set => SetValue(TabItemAccountsCategorySumPositiveNegativeControlProperty, value);
    }

    public static readonly DependencyProperty TabItemBudgetsControlHeaderProperty =
        DependencyProperty.Register(nameof(TabItemBudgetsControlHeader), typeof(string), typeof(AnalyticsPage),
            new PropertyMetadata(default(string)));

    public string TabItemBudgetsControlHeader
    {
        get => (string)GetValue(TabItemBudgetsControlHeaderProperty);
        set => SetValue(TabItemBudgetsControlHeaderProperty, value);
    }

    public AnalyticsPage()
    {
        UpdateLanguage();

        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        TabItemCumulativeSumChartControlHeader = AnalyticsPageResources.TabItemCumulativeSumChartControlHeader;
        TabItemAccountTotalEllipseControlHeader = AnalyticsPageResources.TabItemAccountTotalEllipseControlHeader;
        TabItemAccountsCategorySumControlHeader = AnalyticsPageResources.TabItemAccountsCategorySumControlHeader;
        TabItemAccountsModePaymentMonthlySumControlHeader = AnalyticsPageResources.TabItemAccountsModePaymentMonthlySumControlHeader;
        TabItemAccountsCategorySumPositiveNegativeControl = AnalyticsPageResources.TabItemAccountsCategorySumPositiveNegativeControl;
        TabItemBudgetsControlHeader = AnalyticsPageResources.TabItemBudgetsControlHeader;
    }
}