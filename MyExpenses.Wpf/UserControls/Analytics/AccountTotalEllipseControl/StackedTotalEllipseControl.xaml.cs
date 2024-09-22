using System.Windows;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Wpf.Resources.Resx.UserControls.Analytics.StackedTotalEllipseControl;

namespace MyExpenses.Wpf.UserControls.Analytics.AccountTotalEllipseControl;

public partial class StackedTotalEllipseControl
{
    public static readonly DependencyProperty VTotalByAccountProperty =
        DependencyProperty.Register(nameof(VTotalByAccount), typeof(VTotalByAccount),
            typeof(StackedTotalEllipseControl), new PropertyMetadata(default(VTotalByAccount)));

    public VTotalByAccount VTotalByAccount
    {
        get => (VTotalByAccount)GetValue(VTotalByAccountProperty);
        set => SetValue(VTotalByAccountProperty, value);
    }

    public static readonly DependencyProperty TitleTotalTotalNotPointedProperty =
        DependencyProperty.Register(nameof(TitleTotalTotalNotPointed), typeof(string),
            typeof(StackedTotalEllipseControl), new PropertyMetadata(default(string)));

    public string TitleTotalTotalNotPointed
    {
        get => (string)GetValue(TitleTotalTotalNotPointedProperty);
        set => SetValue(TitleTotalTotalNotPointedProperty, value);
    }

    public static readonly DependencyProperty TitleTotalTotalProperty =
        DependencyProperty.Register(nameof(TitleTotalTotal), typeof(string), typeof(StackedTotalEllipseControl),
            new PropertyMetadata(default(string)));

    public string TitleTotalTotal
    {
        get => (string)GetValue(TitleTotalTotalProperty);
        set => SetValue(TitleTotalTotalProperty, value);
    }

    public static readonly DependencyProperty TitleTotalTotalPointedProperty =
        DependencyProperty.Register(nameof(TitleTotalTotalPointed), typeof(string), typeof(StackedTotalEllipseControl),
            new PropertyMetadata(default(string)));

    public string TitleTotalTotalPointed
    {
        get => (string)GetValue(TitleTotalTotalPointedProperty);
        set => SetValue(TitleTotalTotalPointedProperty, value);
    }

    public StackedTotalEllipseControl()
    {
        UpdateLanguage();

        InitializeComponent();

        Interface.LanguageChanged += InterfaceOnLanguageChanged;
    }

    private void InterfaceOnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        TitleTotalTotalNotPointed = StackedTotalEllipseControlResources.TitleTotalTotalNotPointed;
        TitleTotalTotal = StackedTotalEllipseControlResources.TitleTotalTotal;
        TitleTotalTotalPointed = StackedTotalEllipseControlResources.TitleTotalTotalPointed;
    }
}