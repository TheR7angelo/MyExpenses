using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Views;

namespace MyExpenses.Smartphones.ContentPages.Analytics.AccountTotalEllipseControl;

public partial class StackedTotalEllipseContentView
{
    public static readonly BindableProperty TitleTotalTotalPointedProperty =
        BindableProperty.Create(nameof(TitleTotalTotalPointed), typeof(string), typeof(StackedTotalEllipseContentView));

    public string TitleTotalTotalPointed
    {
        get => (string)GetValue(TitleTotalTotalPointedProperty);
        set => SetValue(TitleTotalTotalPointedProperty, value);
    }

    public static readonly BindableProperty TitleTotalTotalProperty = BindableProperty.Create(nameof(TitleTotalTotal),
        typeof(string), typeof(StackedTotalEllipseContentView));

    public string TitleTotalTotal
    {
        get => (string)GetValue(TitleTotalTotalProperty);
        set => SetValue(TitleTotalTotalProperty, value);
    }

    public static readonly BindableProperty TitleTotalTotalNotPointedProperty =
        BindableProperty.Create(nameof(TitleTotalTotalNotPointed), typeof(string),
            typeof(StackedTotalEllipseContentView));

    public string TitleTotalTotalNotPointed
    {
        get => (string)GetValue(TitleTotalTotalNotPointedProperty);
        set => SetValue(TitleTotalTotalNotPointedProperty, value);
    }

    public static readonly BindableProperty VTotalByAccountProperty = BindableProperty.Create(nameof(VTotalByAccount),
        typeof(VTotalByAccount), typeof(StackedTotalEllipseContentView));

    public VTotalByAccount VTotalByAccount
    {
        get => (VTotalByAccount)GetValue(VTotalByAccountProperty);
        set => SetValue(VTotalByAccountProperty, value);
    }

    public StackedTotalEllipseContentView()
    {
        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += UpdateLanguage;
    }

    private void UpdateLanguage()
    {
        TitleTotalTotalNotPointed = "TitleTotalTotalNotPointed"; //StackedTotalEllipseControlResources.TitleTotalTotalNotPointed;
        TitleTotalTotal = "TitleTotalTotal"; // StackedTotalEllipseControlResources.TitleTotalTotal;
        TitleTotalTotalPointed = "TitleTotalTotalPointed"; // StackedTotalEllipseControlResources.TitleTotalTotalPointed;
    }
}