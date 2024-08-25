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

    public static readonly DependencyProperty TabItemAccountValueTrendControlHeaderProperty =
        DependencyProperty.Register(nameof(TabItemAccountValueTrendControlHeader), typeof(string),
            typeof(AnalyticsPage), new PropertyMetadata(default(string)));

    public string TabItemAccountValueTrendControlHeader
    {
        get => (string)GetValue(TabItemAccountValueTrendControlHeaderProperty);
        set => SetValue(TabItemAccountValueTrendControlHeaderProperty, value);
    }

    public static readonly DependencyProperty TabItemCumulativeSumChartControlHeaderProperty =
        DependencyProperty.Register(nameof(TabItemCumulativeSumChartControlHeader), typeof(string),
            typeof(AnalyticsPage), new PropertyMetadata(default(string)));

    public string TabItemCumulativeSumChartControlHeader
    {
        get => (string)GetValue(TabItemCumulativeSumChartControlHeaderProperty);
        set => SetValue(TabItemCumulativeSumChartControlHeaderProperty, value);
    }

    public static readonly DependencyProperty TabItemCumulativeTotalSumChartControlHeaderProperty =
        DependencyProperty.Register(nameof(TabItemCumulativeTotalSumChartControlHeader), typeof(string),
            typeof(AnalyticsPage), new PropertyMetadata(default(string)));

    public string TabItemCumulativeTotalSumChartControlHeader
    {
        get => (string)GetValue(TabItemCumulativeTotalSumChartControlHeaderProperty);
        set => SetValue(TabItemCumulativeTotalSumChartControlHeaderProperty, value);
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

    public string TabItemAccountsModePaymentMonthlySumControlHeader
    {
        get => (string)GetValue(TabItemAccountsModePaymentMonthlySumControlHeaderProperty);
        set => SetValue(TabItemAccountsModePaymentMonthlySumControlHeaderProperty, value);
    }

    public static readonly DependencyProperty TabItemAccountsCategorySumPositiveNegativeControlProperty =
        DependencyProperty.Register(nameof(TabItemAccountsCategorySumPositiveNegativeControl), typeof(string),
            typeof(AnalyticsPage), new PropertyMetadata(default(string)));

    public string TabItemAccountsCategorySumPositiveNegativeControl
    {
        get => (string)GetValue(TabItemAccountsCategorySumPositiveNegativeControlProperty);
        set => SetValue(TabItemAccountsCategorySumPositiveNegativeControlProperty, value);
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
        TabItemAccountValueTrendControlHeader = AnalyticsPageResources.TabItemAccountValueTrendControlHeader;
        TabItemCumulativeSumChartControlHeader = AnalyticsPageResources.TabItemCumulativeSumChartControlHeader;
        TabItemCumulativeTotalSumChartControlHeader = AnalyticsPageResources.TabItemCumulativeTotalSumChartControlHeader;
        TabItemAccountTotalEllipseControlHeader = AnalyticsPageResources.TabItemAccountTotalEllipseControlHeader;
        TabItemAccountsCategorySumControlHeader = AnalyticsPageResources.TabItemAccountsCategorySumControlHeader;
        TabItemAccountsModePaymentMonthlySumControlHeader = AnalyticsPageResources.TabItemAccountsModePaymentMonthlySumControlHeader;
        TabItemAccountsCategorySumPositiveNegativeControl = "Under working";
    }
}