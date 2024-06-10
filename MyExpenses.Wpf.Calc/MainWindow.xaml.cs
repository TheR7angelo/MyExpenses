using MyExpenses.Wpf.Calc.Calculator;

namespace MyExpenses.Wpf.Calc;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var calculatorWindow = new CalculatorWindow();
        calculatorWindow.ShowDialog();

    }
}