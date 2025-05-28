using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.AddEditLocation;
using MyExpenses.SharedUtils.Resources.Resx.ColorManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditColorWindow
{
    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EditColorProperty =
        DependencyProperty.Register(nameof(EditColor), typeof(bool), typeof(AddEditColorWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EditColor
    {
        get => (bool)GetValue(EditColorProperty);
        set => SetValue(EditColorProperty, value);
    }

    #region Resx

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(AddEditColorWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxColorNameProperty =
        DependencyProperty.Register(nameof(TextBoxColorName), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxColorName
    {
        get => (string)GetValue(TextBoxColorNameProperty);
        set => SetValue(TextBoxColorNameProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonValidContentProperty =
        DependencyProperty.Register(nameof(ButtonValidContent), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonDeleteContentProperty =
        DependencyProperty.Register(nameof(ButtonDeleteContent), typeof(string), typeof(AddEditColorWindow),
            new PropertyMetadata(default(string)));

    public string ButtonDeleteContent
    {
        get => (string)GetValue(ButtonDeleteContentProperty);
        set => SetValue(ButtonDeleteContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
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

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TColor Color { get; } = new();

    public bool DeleteColor { get; private set; }

    public AddEditColorWindow()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        using var context = new DataBaseContext();
        Colors = [..context.TColors];

        UpdateLanguage();
        InitializeComponent();

        this.SetWindowCornerPreference();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void UpdateLanguage()
    {
        TitleWindow = ColorManagementResources.TitleWindow;

        TextBoxColorName = ColorManagementResources.TextBoxColorName;

        ButtonValidContent = ColorManagementResources.ButtonValidContent;
        ButtonCancelContent = ColorManagementResources.ButtonCancelContent;
        ButtonDeleteContent = ColorManagementResources.ButtonDeleteContent;
    }

    #region Action

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Color.Name))
        {
            MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxCannotAddEmptyColorNameError, MsgBoxImage.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(Color.HexadecimalColorCode))
        {
            MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxCannotAddEmptyColorHexError, MsgBoxImage.Error);
            return;
        }

        var nameAlreadyExist = CheckColorName(Color.Name);
        if (nameAlreadyExist)
        {
            ShowErrorMessage();
            return;
        }

        // ReSharper disable once HeapView.DelegateAllocation
        var colorAlreadyExist = Colors.FirstOrDefault(s => s.HexadecimalColorCode == Color.HexadecimalColorCode);
        if (colorAlreadyExist is not null)
        {
            MsgBox.MsgBox.Show(
                string.Format(ColorManagementResources.MessageBoxCannotAddDuplicateColorHexError,
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
        var message = string.Format(ColorManagementResources.MessageBoxDeleteColorQuestionMessage, Color.Name);
        var response = MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteColorQuestionTitle, message,
                MessageBoxButton.YesNoCancel, MsgBoxImage.Question);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the color \"{ColorToDeleteName}\"", Color.Name);
        var (success, exception) = Color.Delete();

        if (success)
        {
            Log.Information("Color was successfully removed");
            MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteColorNoUseSuccess, MsgBoxImage.Check);

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

            response = MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteColorUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information("Attempting to remove the color \"{ColorToDeleteName}\" with all relative element",
                Color.Name);
            Color.Delete(true);
            Log.Information("Account and all relative element was successfully removed");
            MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteColorUseSuccess, MsgBoxImage.Check);

            DeleteColor = true;
            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteAccountError, MsgBoxImage.Error);
    }

    private void Interface_OnLanguageChanged()
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
        var colorToEdit = categoryTypeColorFk.ToISql<TColor>();
        if (colorToEdit is null) return;

        SetTColor(colorToEdit);
    }

    // ReSharper disable once HeapView.ClosureAllocation
    public void SetTColor(TColor colorToEdit)
    {
        colorToEdit.CopyPropertiesTo(Color);
        EditColor = true;

        // ReSharper disable once HeapView.DelegateAllocation
        var removeItem = Colors.FirstOrDefault(s => s.Id == colorToEdit.Id);
        if (removeItem is not null) Colors.Remove(removeItem);
    }

    private static void ShowErrorMessage()
        => MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxCannotAddDuplicateColorNameError,
            MsgBoxImage.Warning);

    #endregion
}