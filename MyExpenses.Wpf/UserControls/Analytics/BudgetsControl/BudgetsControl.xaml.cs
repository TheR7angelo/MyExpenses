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
    }
}