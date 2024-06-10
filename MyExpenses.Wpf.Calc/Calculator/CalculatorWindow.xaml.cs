using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace MyExpenses.Wpf.Calc.Calculator;

public partial class CalculatorWindow
{
    public static readonly DependencyProperty TextCalculationResultProperty =
        DependencyProperty.Register(nameof(TextCalculationResult), typeof(string), typeof(CalculatorWindow),
            new PropertyMetadata(default(string)));

    public string? TextCalculationResult
    {
        get => (string)GetValue(TextCalculationResultProperty);
        set => SetValue(TextCalculationResultProperty, value);
    }

    public CalculatorWindow()
    {
        InitializeComponent();
    }

    private Operator _currentOperator;

    private double _firstNumber;

    private double _secondNumber;

    private void ButtonNumber_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;

        TextCalculationResult = TextCalculationResult is not "0" ? $"{TextCalculationResult}{btn.Content}" : btn.Content.ToString();
    }

    private void ButtonAC_OnClick(object sender, RoutedEventArgs e)
    {
        _firstNumber = 0;
        _secondNumber = 0;
        _currentOperator = 0;
        TextCalculationResult = "0";
    }

    private void ButtonAddition_OnClick(object sender, RoutedEventArgs e)
    {
        _currentOperator = Operator.Add;
        if (!string.IsNullOrWhiteSpace(TextCalculationResult)) _firstNumber = double.Parse(TextCalculationResult);
        TextCalculationResult = "0";
    }

    private void ButtonEqual_OnClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TextCalculationResult)) _secondNumber = double.Parse(TextCalculationResult);
        TextCalculationResult = GetResult(_firstNumber, _currentOperator, _secondNumber);
    }

    private string GetResult(double firstNumber, Operator currentOperator, double secondNumber)
    {
        return currentOperator switch
        {
            Operator.Add => (firstNumber + secondNumber).ToString(CultureInfo.InvariantCulture),
            Operator.Subtract => (firstNumber - secondNumber).ToString(CultureInfo.InvariantCulture),
            Operator.Multiply => (firstNumber * secondNumber).ToString(CultureInfo.InvariantCulture),
            Operator.Divide => (firstNumber / secondNumber).ToString(CultureInfo.InvariantCulture),
            _ => "0"
        };
    }

    private void ButtonMinus_OnClick(object sender, RoutedEventArgs e)
    {
        _currentOperator = Operator.Subtract;
        if (!string.IsNullOrWhiteSpace(TextCalculationResult)) _firstNumber = double.Parse(TextCalculationResult);
        TextCalculationResult = "0";
    }

    private void ButtonMultiply_OnClick(object sender, RoutedEventArgs e)
    {
        _currentOperator = Operator.Multiply;
        if (!string.IsNullOrWhiteSpace(TextCalculationResult)) _firstNumber = double.Parse(TextCalculationResult);
        TextCalculationResult = "0";
    }

    private void ButtonDivide_OnClick(object sender, RoutedEventArgs e)
    {
        _currentOperator = Operator.Divide;
        if (!string.IsNullOrWhiteSpace(TextCalculationResult)) _firstNumber = double.Parse(TextCalculationResult);
        TextCalculationResult = "0";
    }

    private void ButtonReversePolarity_OnClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TextCalculationResult))
        {
            TextCalculationResult = (double.Parse(TextCalculationResult) * -1).ToString(CultureInfo.InvariantCulture);
        }
    }

    private void ButtonPercentage_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
    }

    private void ButtonComma_OnClick(object sender, RoutedEventArgs e)
    {
        if (TextCalculationResult?.IndexOf('.') < 0)
        {
            TextCalculationResult += ".";
        }
    }
}

public enum Operator
{
    Add,
    Subtract,
    Multiply,
    Divide
}
