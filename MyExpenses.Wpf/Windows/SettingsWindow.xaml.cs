using System.Windows.Input;

namespace MyExpenses.Wpf.Windows;

public partial class SettingsWindow
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        Console.WriteLine("hey");
    }
}