using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
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

    public static readonly DependencyProperty TextBoxColorNameProperty =
        DependencyProperty.Register(nameof(TextBoxColorName), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxColorName
    {
        get => (string)GetValue(TextBoxColorNameProperty);
        set => SetValue(TextBoxColorNameProperty, value);
    }

    public static readonly DependencyProperty LabelRedChannelProperty =
        DependencyProperty.Register(nameof(LabelRedChannel), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string LabelRedChannel
    {
        get => (string)GetValue(LabelRedChannelProperty);
        set => SetValue(LabelRedChannelProperty, value);
    }

    public static readonly DependencyProperty LabelGreenChannelProperty =
        DependencyProperty.Register(nameof(LabelGreenChannel), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string LabelGreenChannel
    {
        get => (string)GetValue(LabelGreenChannelProperty);
        set => SetValue(LabelGreenChannelProperty, value);
    }

    public static readonly DependencyProperty LabelBlueChannelProperty =
        DependencyProperty.Register(nameof(LabelBlueChannel), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string LabelBlueChannel
    {
        get => (string)GetValue(LabelBlueChannelProperty);
        set => SetValue(LabelBlueChannelProperty, value);
    }

    public static readonly DependencyProperty LabelHueChannelProperty =
        DependencyProperty.Register(nameof(LabelHueChannel), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string LabelHueChannel
    {
        get => (string)GetValue(LabelHueChannelProperty);
        set => SetValue(LabelHueChannelProperty, value);
    }

    public static readonly DependencyProperty LabelSaturationChannelProperty =
        DependencyProperty.Register(nameof(LabelSaturationChannel), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string LabelSaturationChannel
    {
        get => (string)GetValue(LabelSaturationChannelProperty);
        set => SetValue(LabelSaturationChannelProperty, value);
    }

    public static readonly DependencyProperty LabelValueChannelProperty =
        DependencyProperty.Register(nameof(LabelValueChannel), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string LabelValueChannel
    {
        get => (string)GetValue(LabelValueChannelProperty);
        set => SetValue(LabelValueChannelProperty, value);
    }

    public static readonly DependencyProperty LabelAlphaChannelProperty =
        DependencyProperty.Register(nameof(LabelAlphaChannel), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string LabelAlphaChannel
    {
        get => (string)GetValue(LabelAlphaChannelProperty);
        set => SetValue(LabelAlphaChannelProperty, value);
    }

    public static readonly DependencyProperty LabelHexadecimalCodeProperty =
        DependencyProperty.Register(nameof(LabelHexadecimalCode), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string LabelHexadecimalCode
    {
        get => (string)GetValue(LabelHexadecimalCodeProperty);
        set => SetValue(LabelHexadecimalCodeProperty, value);
    }

    public static readonly DependencyProperty LabelPreviewProperty = DependencyProperty.Register(nameof(LabelPreview),
        typeof(string), typeof(AddEditColorWindow), new PropertyMetadata(default(string)));

    public string LabelPreview
    {
        get => (string)GetValue(LabelPreviewProperty);
        set => SetValue(LabelPreviewProperty, value);
    }

    public static readonly DependencyProperty ButtonValidContentProperty =
        DependencyProperty.Register(nameof(ButtonValidContent), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    public static readonly DependencyProperty ButtonDeleteContentProperty =
        DependencyProperty.Register(nameof(ButtonDeleteContent), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string ButtonDeleteContent
    {
        get => (string)GetValue(ButtonDeleteContentProperty);
        set => SetValue(ButtonDeleteContentProperty, value);
    }

    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    #endregion

    private List<TColor> Colors { get; }

    public TColor Color { get; } = new();

    public bool DeleteColor { get; private set; }

    public AddEditColorWindow()
    {
        using var context = new DataBaseContext();
        Colors = [..context.TColors];

        Interface.LanguageChanged += Interface_OnLanguageChanged;
        UpdateLanguage();
        InitializeComponent();

        var hWnd = new WindowInteropHelper(GetWindow(this)!).EnsureHandle();
        hWnd.SetWindowCornerPreference(DwmWindowCornerPreference.Round);
    }

    private void UpdateLanguage()
    {
        LabelRedChannel = AddEditColorWindowResources.LabelRedChannel;
        LabelGreenChannel = AddEditColorWindowResources.LabelGreenChannel;
        LabelBlueChannel = AddEditColorWindowResources.LabelBlueChannel;
        LabelHueChannel = AddEditColorWindowResources.LabelHueChannel;
        LabelSaturationChannel = AddEditColorWindowResources.LabelSaturationChannel;
        LabelValueChannel = AddEditColorWindowResources.LabelValueChannel;
        LabelAlphaChannel = AddEditColorWindowResources.LabelAlphaChannel;
        LabelPreview = AddEditColorWindowResources.LabelPreview;
        LabelHexadecimalCode = AddEditColorWindowResources.LabelHexadecimalCode;

        TextBoxColorName = AddEditColorWindowResources.TextBoxColorName;

        ButtonValidContent = AddEditColorWindowResources.ButtonValidContent;
        ButtonCancelContent = AddEditColorWindowResources.ButtonCancelContent;
        ButtonDeleteContent = AddEditColorWindowResources.ButtonDeleteContent;
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
            MsgBox.MsgBox.Show(
                string.Format(AddEditColorWindowResources.MessageBoxCannotAddDuplicateColorHexError,
                    colorAlreadyExist.Name),
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

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

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