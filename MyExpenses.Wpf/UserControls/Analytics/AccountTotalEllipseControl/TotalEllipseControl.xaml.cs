using System.Windows;

namespace MyExpenses.Wpf.UserControls.Analytics.AccountTotalEllipseControl;

public partial class TotalEllipseControl
{
    public static readonly DependencyProperty TotalProperty = DependencyProperty.Register(nameof(Total), typeof(double),
        // ReSharper disable once HeapView.BoxingAllocation
        typeof(TotalEllipseControl), new PropertyMetadata(0d));

    public double? Total
    {
        get => (double)GetValue(TotalProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(TotalProperty, value);
    }

    public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(nameof(Symbol),
        typeof(string), typeof(TotalEllipseControl), new PropertyMetadata(default(string)));

    public string Symbol
    {
        get => (string)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    public static readonly DependencyProperty TitleTotalProperty = DependencyProperty.Register(nameof(TitleTotal),
        typeof(string), typeof(TotalEllipseControl), new PropertyMetadata(default(string)));

    public string TitleTotal
    {
        get => (string)GetValue(TitleTotalProperty);
        set => SetValue(TitleTotalProperty, value);
    }

    public TotalEllipseControl()
    {
        InitializeComponent();
    }
}