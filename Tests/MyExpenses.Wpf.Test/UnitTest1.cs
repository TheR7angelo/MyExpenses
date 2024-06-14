using System.Windows;
using System.Windows.Controls;

namespace MyExpenses.Wpf.Test;

public class UnitTest1
{
    [StaFact]
    public void Test1()
    {
        if (Application.Current is null) new Application();

        if (Application.Current?.TryFindResource("BooleanToVisibilityConverter") is null)
        {
            var booleanToVisibilityConverter = new BooleanToVisibilityConverter();
            Application.Current?.Resources.Add("BooleanToVisibilityConverter", booleanToVisibilityConverter);
        }

        var mainWindow = new MainWindow();
    }
}