using System.Windows;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Wpf.Resources.Resx.UserControls.Analytics.BudgetsControl;

namespace MyExpenses.Wpf.UserControls.Analytics.BudgetsControl;

public partial class BudgetsControl
{
    public static readonly DependencyProperty BudgetMonthlyControlHeaderProperty =
        DependencyProperty.Register(nameof(BudgetMonthlyControlHeader), typeof(string), typeof(BudgetsControl),
            new PropertyMetadata(default(string)));

    public string BudgetMonthlyControlHeader
    {
        get => (string)GetValue(BudgetMonthlyControlHeaderProperty);
        set => SetValue(BudgetMonthlyControlHeaderProperty, value);
    }

    public static readonly DependencyProperty BudgetAnnualControlHeaderProperty =
        DependencyProperty.Register(nameof(BudgetAnnualControlHeader), typeof(string), typeof(BudgetsControl),
            new PropertyMetadata(default(string)));

    public string BudgetAnnualControlHeader
    {
        get => (string)GetValue(BudgetAnnualControlHeaderProperty);
        set => SetValue(BudgetAnnualControlHeaderProperty, value);
    }

    public BudgetsControl()
    {
        UpdateLanguage();

        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        BudgetMonthlyControlHeader = BudgetsControlResources.BudgetMonthlyControlHeader;
        BudgetAnnualControlHeader = BudgetsControlResources.BudgetAnnualControlHeader;
    }
}