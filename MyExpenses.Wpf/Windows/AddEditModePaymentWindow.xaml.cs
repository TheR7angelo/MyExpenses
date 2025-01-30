using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditCurrencyWindow;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditModePaymentWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditModePaymentWindow
{
    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EditModePaymentProperty =
        DependencyProperty.Register(nameof(EditModePayment), typeof(bool), typeof(AddEditModePaymentWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TModePayment ModePayment { get; } = new();

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EditModePayment
    {
        get => (bool)GetValue(EditModePaymentProperty);
        set => SetValue(EditModePaymentProperty, value);
    }

    private List<TModePayment> ModePayments { get; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxModePaymentNameProperty =
        DependencyProperty.Register(nameof(TextBoxModePaymentName), typeof(string), typeof(AddEditModePaymentWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxModePaymentName
    {
        get => (string)GetValue(TextBoxModePaymentNameProperty);
        set => SetValue(TextBoxModePaymentNameProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonValidContentProperty =
        DependencyProperty.Register(nameof(ButtonValidContent), typeof(string), typeof(AddEditModePaymentWindow),
            new PropertyMetadata(default(string)));

    public string ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonDeleteContentProperty =
        DependencyProperty.Register(nameof(ButtonDeleteContent), typeof(string), typeof(AddEditModePaymentWindow),
            new PropertyMetadata(default(string)));

    public string ButtonDeleteContent
    {
        get => (string)GetValue(ButtonDeleteContentProperty);
        set => SetValue(ButtonDeleteContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(AddEditModePaymentWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty WindowTitleProperty = DependencyProperty.Register(nameof(WindowTitle),
        typeof(string), typeof(AddEditModePaymentWindow), new PropertyMetadata(default(string)));

    public string WindowTitle
    {
        get => (string)GetValue(WindowTitleProperty);
        set => SetValue(WindowTitleProperty, value);
    }

    public bool ModePaymentDeleted { get; private set; }

    public AddEditModePaymentWindow()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        using var context = new DataBaseContext();
        ModePayments = [..context.TModePayments];

        Interface.LanguageChanged += Interface_OnLanguageChanged;
        UpdateLanguage();
        InitializeComponent();

        this.SetWindowCornerPreference();

        TextBoxModePayment.Focus();
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        WindowTitle = AddEditModePaymentWindowResources.WindowTitle;

        TextBoxModePaymentName = AddEditModePaymentWindowResources.TextBoxModePaymentName;
        ButtonValidContent = AddEditModePaymentWindowResources.ButtonValidContent;
        ButtonDeleteContent = AddEditModePaymentWindowResources.ButtonDeleteContent;
        ButtonCancelContent = AddEditModePaymentWindowResources.ButtonCancelContent;
    }

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.MsgBox.Show(AddEditModePaymentWindowResources.MessageBoxDeleteQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the currency symbol \"{ModePaymentName}\"", ModePayment.Name);
        var (success, exception) = ModePayment.Delete();

        if (success)
        {
            Log.Information("Mode payment was successfully removed");
            MsgBox.MsgBox.Show(AddEditModePaymentWindowResources.MessageBoxDeleteModePaymentNoUseSuccess,
                MsgBoxImage.Check);

            ModePaymentDeleted = true;
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

            response = MsgBox.MsgBox.Show(AddEditModePaymentWindowResources.MessageBoxDeleteModePaymentUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information("Attempting to remove the mode payment \"{ModePaymentName}\" with all relative element",
                ModePayment.Name);
            ModePayment.Delete(true);
            Log.Information("Mode payment and all relative element was successfully removed");
            MsgBox.MsgBox.Show(AddEditModePaymentWindowResources.MessageBoxDeleteModePaymentUseSuccess,
                MsgBoxImage.Check);

            ModePaymentDeleted = true;
            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.MsgBox.Show(AddEditModePaymentWindowResources.MessageBoxDeleteModePaymentError, MsgBoxImage.Error);
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var modePaymentName = ModePayment.Name;

        if (string.IsNullOrWhiteSpace(modePaymentName))
        {
            MsgBox.MsgBox.Show(AddEditModePaymentWindowResources.MessageBoxModePaymentNameEmptyError,
                MsgBoxImage.Error);
            return;
        }

        var alreadyExist = CheckModePaymentName(modePaymentName);
        if (alreadyExist) ShowErrorMessage();
        else
        {
            DialogResult = true;
            Close();
        }
    }

    private void TextBoxModePayment_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        var modePaymentName = textBox.Text;
        if (string.IsNullOrEmpty(modePaymentName)) return;

        var alreadyExist = CheckModePaymentName(modePaymentName);
        if (alreadyExist) ShowErrorMessage();
    }

    #endregion

    #region Function

    private bool CheckModePaymentName(string modePaymentName)
        => ModePayments.Select(s => s.Name).Contains(modePaymentName);

    public void SetTModePayment(TModePayment oldModePayment)
    {
        oldModePayment.CopyPropertiesTo(ModePayment);
        EditModePayment = true;

        var modePaymentToRemove = ModePayments.FirstOrDefault(s => s.Id == oldModePayment.Id);
        if (modePaymentToRemove is not null) ModePayments.Remove(modePaymentToRemove);
    }

    private static void ShowErrorMessage()
        => MsgBox.MsgBox.Show(AddEditCurrencyWindowResources.MessageBoxCurrencySymbolAlreadyExists,
            MsgBoxImage.Warning);

    #endregion
}