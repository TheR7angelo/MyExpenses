using System.Windows;
using System.Windows.Media;
using MyExpenses.Wpf.Resources.Resx.Windows.ColorPickerWindow;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class ColorPickerWindow
{
    public string ButtonValidContent { get; } = ColorPickerWindowResources.ButtonValidContent;
    public string ButtonCancelContent { get; } = ColorPickerWindowResources.ButtonCancelContent;

    public Color? ColorResult { get; private set; }

    public ColorPickerWindow()
    {
        InitializeComponent();

        this.SetWindowCornerPreference();
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