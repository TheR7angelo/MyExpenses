using System.Windows;

namespace MyExpenses.Wpf.Test.Calculator;

public partial class CalculatorWindow
{
    public static readonly DependencyProperty TextCalculationResultProperty =
        DependencyProperty.Register(nameof(TextCalculationResult), typeof(string), typeof(CalculatorWindow),
            new PropertyMetadata(default(string)));

    public string TextCalculationResult
    {
        get => (string)GetValue(TextCalculationResultProperty);
        set => SetValue(TextCalculationResultProperty, value);
    }

    public static readonly DependencyProperty CalculationResultProperty =
        DependencyProperty.Register(nameof(CalculationResult), typeof(double), typeof(CalculatorWindow),
            new PropertyMetadata(default(double), PropertyChangedCallback));

    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Console.WriteLine(e.NewValue);
    }

    public double CalculationResult
    {
        get => (double)GetValue(CalculationResultProperty);
        set => SetValue(CalculationResultProperty, value);
    }

    public CalculatorWindow()
    {
        InitializeComponent();
    }

    private void ButtonReversePolarity_OnClick(object sender, RoutedEventArgs e)
    {
        CalculationResult = - CalculationResult;
    }
}