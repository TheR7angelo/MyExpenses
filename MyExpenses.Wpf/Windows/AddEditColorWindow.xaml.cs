using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditColorWindow;
using MyExpenses.Wpf.UserControls;
using MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

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
    public required AddEditCategoryTypeWindow AddEditCategoryType { get; set; }

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
        if (string.IsNullOrWhiteSpace(Color.Name))
        {
            MsgBox.MsgBox.Show(AddEditColorWindowResources.MessageBoxCannotAddEmptyColorNameError, MsgBoxImage.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(Color.HexadecimalColorCode))
        {
            MsgBox.MsgBox.Show(AddEditColorWindowResources.MessageBoxCannotAddEmptyColorHexError, MsgBoxImage.Error);
            return;
        }

        var nameAlreadyExist = CheckColorName(Color.Name);
        if (nameAlreadyExist)
        {
            ShowErrorMessage();
            return;
        }

        var colorAlreadyExist = Colors.FirstOrDefault(s => s.HexadecimalColorCode == Color.HexadecimalColorCode);
        if (colorAlreadyExist is not null)
        {
            MsgBox.MsgBox.Show(string.Format(AddEditColorWindowResources.MessageBoxCannotAddDuplicateColorHexError, colorAlreadyExist.Name),
                MsgBoxImage.Error);
            return;
        }

        Log.Information("Attempt to inject the new color \"{ColorName}\" with hexadecimal code \"{ColorHexadecimalColorCode}\"",
            Color.Name, Color.HexadecimalColorCode);

        var (success, exception) = Color.AddOrEdit();
        if (success)
        {
            Log.Information("color was successfully added");
            MsgBox.MsgBox.Show(AddEditColorWindowResources.MessageBoxAddColorSuccess, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.MsgBox.Show(AddEditColorWindowResources.MessageBoxAddColorError, MsgBoxImage.Error);
        }
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
        Console.WriteLine(Color.Name);
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
        => MsgBox.MsgBox.Show(AddEditColorWindowResources.MessageBoxCannotAddDuplicateColorNameError, MsgBoxImage.Warning);

    #endregion
}