using System.Windows;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;

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

    public string TextBoxModePaymentName { get; } = "Mode payment name :";
    public string ButtonValidContent { get; } = "Valid";
    public string ButtonDeleteContent { get; } = "Delete";
    public string ButtonCancelContent { get; } = "Cancel";


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
        //TODO work
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void TextBoxModePayment_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        //TODO work
    }

    #endregion
}