namespace MyExpenses.Smartphones.ContentPages.Analytics.AccountTotalEllipseControl;

public partial class TotalEllipseContentView
{
    public static readonly BindableProperty SymbolProperty = BindableProperty.Create(nameof(Symbol), typeof(string),
        typeof(TotalEllipseContentView));

    public string Symbol
    {
        get => (string)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    public static readonly BindableProperty TotalProperty = BindableProperty.Create(nameof(Total), typeof(double),
        typeof(TotalEllipseContentView), 0d);

    public double? Total
    {
        get => (double?)GetValue(TotalProperty);
        set => SetValue(TotalProperty, value);
    }

    public static readonly BindableProperty TitleTotalProperty = BindableProperty.Create(nameof(TitleTotal),
        typeof(string), typeof(TotalEllipseContentView));

    public string TitleTotal
    {
        get => (string)GetValue(TitleTotalProperty);
        set => SetValue(TitleTotalProperty, value);
    }

    public TotalEllipseContentView()
    {
        InitializeComponent();
    }
}