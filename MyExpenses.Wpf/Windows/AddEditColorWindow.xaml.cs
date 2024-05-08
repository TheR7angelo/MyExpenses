using MyExpenses.Wpf.UserControls;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditColorWindow
{
    public AddEditColorWindow()
    {
        InitializeComponent();
    }

    private void ColorPickerControl_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        Console.WriteLine(e.HexadecimalCode);
    }
}