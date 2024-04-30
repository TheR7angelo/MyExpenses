using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.AddEditCurrencyWindow;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditCurrencyWindow
{
    #region Property

    public TCurrency Currency { get; } = new();

    private List<TCurrency> Currencies { get; }

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

    #region Function

    private bool CheckCurrencySymbol(string accountName)
        => Currencies.Select(s => s.Symbol).Contains(accountName);

    private void ShowErrorMessage()
        => MessageBox.Show(AddEditCurrencyWindowResources.MessageBoxCurrencySymbolAlreadyExists);

    #endregion

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
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
}