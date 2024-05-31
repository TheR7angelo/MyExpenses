using MyExpenses.Wpf.Test.Calculator;

namespace MyExpenses.Wpf.Test;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var calculatorWindow = new CalculatorWindow();
        calculatorWindow.ShowDialog();

    }
}