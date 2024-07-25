using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.WindowStyle;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditColorWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditColorWindow
{
    public static readonly DependencyProperty EditColorProperty =
        DependencyProperty.Register(nameof(EditColor), typeof(bool), typeof(AddEditColorWindow),
            new PropertyMetadata(default(bool)));

    public bool EditColor
    {
        get => (bool)GetValue(EditColorProperty);
        set => SetValue(EditColorProperty, value);
    }

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

    public TColor Color { get; } = new();

    public bool DeleteColor { get; private set; }

    //TODO add language
    public AddEditColorWindow()
    {
        using var context = new DataBaseContext();
        Colors = [..context.TColors];

        InitializeComponent();

        var hWnd = new WindowInteropHelper(GetWindow(this)!).EnsureHandle();
        hWnd.SetWindowCornerPreference(DwmWindowCornerPreference.Round);
    }

    #region Action

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
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

        DialogResult = true;
        Close();
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response =
            MsgBox.MsgBox.Show(string.Format(AddEditColorWindowResources.MessageBoxDeleteColorQuestion, Color.Name),
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the color \"{ColorToDeleteName}\"", Color.Name);
        var (success, exception) = Color.Delete();

        if (success)
        {
            Log.Information("Color was successfully removed");
            MsgBox.MsgBox.Show(AddEditColorWindowResources.MessageBoxDeleteColorNoUseSuccess, MsgBoxImage.Check);

            DeleteColor = true;
            DialogResult = true;
            Close();
            return;
        }

        if (exception!.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
            })
        {
            Log.Error("Foreign key constraint violation");

            response = MsgBox.MsgBox.Show(AddEditColorWindowResources.MessageBoxDeleteColorUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information("Attempting to remove the color \"{ColorToDeleteName}\" with all relative element",
                Color.Name);
            Color.Delete(true);
            Log.Information("Account and all relative element was successfully removed");
            MsgBox.MsgBox.Show(AddEditColorWindowResources.MessageBoxDeleteColorUseSuccess, MsgBoxImage.Check);

            DeleteColor = true;
            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.MsgBox.Show(AddEditColorWindowResources.MessageBoxDeleteAccountError, MsgBoxImage.Error);
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

    public void SetTColor(int categoryTypeColorFk)
    {
        var colorToEdit = categoryTypeColorFk.ToISqlT<TColor>();
        if (colorToEdit is null) return;

        SetTColor(colorToEdit);
    }

    public void SetTColor(TColor colorToEdit)
    {
        colorToEdit.CopyPropertiesTo(Color);
        EditColor = true;

        var removeItem = Colors.FirstOrDefault(s => s.Id == colorToEdit.Id);
        if (removeItem is not null) Colors.Remove(removeItem);
    }

    private void ShowErrorMessage()
        => MsgBox.MsgBox.Show(AddEditColorWindowResources.MessageBoxCannotAddDuplicateColorNameError,
            MsgBoxImage.Warning);

    #endregion

}