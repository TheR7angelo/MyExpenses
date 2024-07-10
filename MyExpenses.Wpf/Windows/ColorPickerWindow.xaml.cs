using System.Windows;
using System.Windows.Media;

namespace MyExpenses.Wpf.Windows;

public partial class ColorPickerWindow
{
    public Color? ColorResult { get; private set; }

    public ColorPickerWindow()
    {
        InitializeComponent();
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        ColorResult = ColorPickerControl.Color;
        DialogResult = true;

        Close();
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;

        Close();
    }
}