using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditColorWindow;
using MyExpenses.Wpf.UserControls;
using MyExpenses.Wpf.Windows.MsgBox;

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

    private List<TColor> Colors { get; }

    public TColor Color { get; private set; } = new();

    public AddEditColorWindow()
    {
        using var context = new DataBaseContext();
        Colors = [..context.TColors];

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

    private void UIElement_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        var colorName = textBox.Text;
        if (string.IsNullOrEmpty(colorName)) return;

        var alreadyExist = CheckColorName(colorName);
        if (alreadyExist) ShowErrorMessage();
    }

    #endregion

    #region Function

    private bool CheckColorName(string accountName)
        => Colors.Select(s => s.Name).Contains(accountName);

    private void ShowErrorMessage()
        => MsgBox.MsgBox.Show(AddEditColorWindowResources.MessageBoxColorNameAlreadyExists, MsgBoxImage.Warning);

    #endregion
}