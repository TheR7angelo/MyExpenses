using System.Windows;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditColorWindow;
using MyExpenses.Wpf.UserControls;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditColorWindow
{

    #region Resx

    public string LabelRedChannel { get; } = AddEditColorWindowResources.LabelRedChannel;
    public string LabelGreenChannel { get; } = AddEditColorWindowResources.LabelGreenChannel;
    public string LabelBlueChannel { get; } = AddEditColorWindowResources.LabelBlueChannel;
    public string LabelHueChannel { get; } = AddEditColorWindowResources.LabelHueChannel;
    public string LabelSaturationChannel { get; } = AddEditColorWindowResources.LabelSaturationChannel;
    public string LabelValueChannel { get; } = AddEditColorWindowResources.LabelValueChannel;
    public string LabelAlphaChannel { get; } = AddEditColorWindowResources.LabelAlphaChannel;
    public string LabelPreview { get; } = AddEditColorWindowResources.LabelPreview;
    public string LabelHexadecimalCode { get; } = AddEditColorWindowResources.LabelHexadecimalCode;

    public string TextBoxColorName { get; } = AddEditColorWindowResources.TextBoxColorName;

    public string ButtonValidContent { get; } = AddEditColorWindowResources.ButtonValidContent;
    public string ButtonCancelContent { get; } = AddEditColorWindowResources.ButtonCancelContent;
    public string ButtonDeleteContent { get; } = AddEditColorWindowResources.ButtonDeleteContent;

    #endregion

    public TColor Color { get; private set; } = new();

    public AddEditColorWindow()
    {
        InitializeComponent();
    }

    #region Action

    private void ColorPickerControl_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        //TODO work
        Console.WriteLine(e.HexadecimalCode);
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
        Console.WriteLine("Valid");
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
        Console.WriteLine("Cancel");
    }

    #endregion
}