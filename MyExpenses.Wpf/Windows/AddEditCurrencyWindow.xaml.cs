using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditCurrencyWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditCurrencyWindow
{
    #region DepencyProperty

    public static readonly DependencyProperty EditCurrencyProperty = DependencyProperty.Register(nameof(EditCurrency),
        typeof(bool), typeof(AddEditCurrencyWindow), new PropertyMetadata(default(bool)));

    public bool EditCurrency
    {
        get => (bool)GetValue(EditCurrencyProperty);
        set => SetValue(EditCurrencyProperty, value);
    }

    #endregion

    #region Property

    public TCurrency Currency { get; } = new();

    private List<TCurrency> Currencies { get; }

    public bool CurrencyDeleted { get; set; }

    #endregion

    #region Resx

    public string TextBoxCurrencySymbol { get; } = AddEditCurrencyWindowResources.TextBoxCurrencySymbol;
    public string ButtonValidContent { get; } = AddEditCurrencyWindowResources.ButtonValidContent;
    public string ButtonCancelContent { get; } = AddEditCurrencyWindowResources.ButtonCancelContent;

    #endregion

    public AddEditCurrencyWindow()
    {
        using var context = new DataBaseContext();
        Currencies = [..context.TCurrencies];

        InitializeComponent();
        TextBoxCurrency.Focus();
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
        var currencySymbol = Currency.Symbol;

        if (string.IsNullOrEmpty(currencySymbol)) return;

        var alreadyExist = CheckCurrencySymbol(currencySymbol);
        if (alreadyExist) ShowErrorMessage();
        else
        {
            DialogResult = true;
            Close();
        }
    }

    private void TextBoxCurrencySymbol_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        var currencySymbol = textBox.Text;
        if (string.IsNullOrEmpty(currencySymbol)) return;

        var alreadyExist = CheckCurrencySymbol(currencySymbol);
        if (alreadyExist) ShowErrorMessage();
    }

    #endregion

    #region Function

    private bool CheckCurrencySymbol(string accountName)
        => Currencies.Select(s => s.Symbol).Contains(accountName);

    public void SetTCurrency(TCurrency currencyToEdit)
    {
        currencyToEdit.CopyPropertiesTo(Currency);
        EditCurrency = true;

        var oldItem = Currencies.FirstOrDefault(s => s.Id == currencyToEdit.Id);
        if (oldItem is null) return;
        Currencies.Remove(oldItem);
    }

    private void ShowErrorMessage()
        => MsgBox.MsgBox.Show(AddEditCurrencyWindowResources.MessageBoxCurrencySymbolAlreadyExists,
            MsgBoxImage.Warning);

    #endregion
}