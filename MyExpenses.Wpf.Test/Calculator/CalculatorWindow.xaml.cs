using System.Windows;

namespace MyExpenses.Wpf.Test.Calculator;

public partial class CalculatorWindow
{
    public static readonly DependencyProperty TextCalculationResultProperty =
        DependencyProperty.Register(nameof(TextCalculationResult), typeof(string), typeof(CalculatorWindow),
            new PropertyMetadata(default(string)));

    public CalculatorWindow()
    {
        InitializeComponent();
    }

    public string TextCalculationResult
    {
        get => (string)GetValue(TextCalculationResultProperty);
        set => SetValue(TextCalculationResultProperty, value);
    }
}