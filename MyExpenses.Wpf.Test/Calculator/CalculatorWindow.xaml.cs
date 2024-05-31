using System.Globalization;
using System.Windows;
using System.Windows.Controls;

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

    public CalculatorWindow()
    {
        InitializeComponent();
    }

    private Operator _currentOperator;

    private double _firstNumber;

    private double _secondNumber;

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;

        txtResult.Text = txtResult.Text is not "0" ? $"{txtResult.Text}{btn.Content}" : btn.Content.ToString();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        _firstNumber = 0;
        _secondNumber = 0;
        _currentOperator = 0;
        txtResult.Text = "0";
    }

    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        _currentOperator = Operator.Add;
        _firstNumber = double.Parse(txtResult.Text);
        txtResult.Text = "0";
    }

    private void Button_Click_3(object sender, RoutedEventArgs e)
    {
        _secondNumber = double.Parse(txtResult.Text);
        txtResult.Text = GetResult(_firstNumber, _currentOperator, _secondNumber);
    }

    private string GetResult(double firstNumber, Operator currentOperator, double secondNumber)
    {
        if (currentOperator == Operator.Add)
        {
            return (firstNumber + secondNumber).ToString(CultureInfo.InvariantCulture);
        }
        else if (currentOperator == Operator.Subtract)
        {
            return (firstNumber - secondNumber).ToString(CultureInfo.InvariantCulture);
        }
        else if (currentOperator == Operator.Multiply)
        {
            return (firstNumber * secondNumber).ToString(CultureInfo.InvariantCulture);
        }
        else if (currentOperator == Operator.Divide)
        {
            return (firstNumber / secondNumber).ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            return "0";
        }
    }

    private void Button_Click_4(object sender, RoutedEventArgs e)
    {
        _currentOperator = Operator.Subtract;
        _firstNumber = double.Parse(txtResult.Text);
        txtResult.Text = "0";
    }

    private void Button_Click_5(object sender, RoutedEventArgs e)
    {
        _currentOperator = Operator.Multiply;
        _firstNumber = double.Parse(txtResult.Text);
        txtResult.Text = "0";
    }

    private void Button_Click_6(object sender, RoutedEventArgs e)
    {
        _currentOperator = Operator.Divide;
        _firstNumber = double.Parse(txtResult.Text);
        txtResult.Text = "0";
    }

    private void Button_Click_7(object sender, RoutedEventArgs e)
    {
        txtResult.Text = (double.Parse(txtResult.Text) * -1).ToString(CultureInfo.InvariantCulture);
    }

    private void Button_Click_8(object sender, RoutedEventArgs e)
    {
    }

    private void Button_Click_9(object sender, RoutedEventArgs e)
    {
        if (txtResult.Text.IndexOf('.') < 0)
        {
            txtResult.Text += ".";
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
