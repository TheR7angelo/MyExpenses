using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditCurrencyWindow;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditModePaymentWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditModePaymentWindow
{
    public static readonly DependencyProperty EditModePaymentProperty =
        DependencyProperty.Register(nameof(EditModePayment), typeof(bool), typeof(AddEditModePaymentWindow),
            new PropertyMetadata(default(bool)));

    public TModePayment ModePayment { get; } = new();

    public bool EditModePayment
    {
        get => (bool)GetValue(EditModePaymentProperty);
        set => SetValue(EditModePaymentProperty, value);
    }

    private List<TModePayment> ModePayments { get; }

    public string TextBoxModePaymentName { get; } = AddEditModePaymentWindowResources.TextBoxModePaymentName;
    public string ButtonValidContent { get; } = AddEditModePaymentWindowResources.ButtonValidContent;
    public string ButtonDeleteContent { get; } = AddEditModePaymentWindowResources.ButtonDeleteContent;
    public string ButtonCancelContent { get; } = AddEditModePaymentWindowResources.ButtonCancelContent;
    public bool ModePaymentDeleted { get; private set; }


    public AddEditModePaymentWindow()
    {
        using var context = new DataBaseContext();
        ModePayments = [..context.TModePayments];

        InitializeComponent();
        TextBoxModePayment.Focus();
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
            MsgBox.MsgBox.Show(AddEditModePaymentWindowResources.MessageBoxDeleteModePaymentNoUseSuccess, MsgBoxImage.Check);

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
            MsgBox.MsgBox.Show(AddEditModePaymentWindowResources.MessageBoxDeleteModePaymentUseSuccess, MsgBoxImage.Check);

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

        if (string.IsNullOrEmpty(modePaymentName)) return;

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

    private void ShowErrorMessage()
        => MsgBox.MsgBox.Show(AddEditCurrencyWindowResources.MessageBoxCurrencySymbolAlreadyExists,
            MsgBoxImage.Warning);

    #endregion
}