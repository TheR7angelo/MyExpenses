using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Resources.Resx.Pages.CurrencyManagementPage;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class CurrencyManagementPage
{
    public ObservableCollection<TCurrency> Currencies { get; }
    public required DashBoardPage DashBoardPage { get; set; }

    public CurrencyManagementPage()
    {
        using var context = new DataBaseContext();
        Currencies = [..context.TCurrencies.OrderBy(s => s.Symbol)];

        InitializeComponent();
    }

    private void ButtonAddCurrency_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditCurrency = new AddEditCurrencyWindow();
        var result = addEditCurrency.ShowDialog();
        if (result != true) return;

        var newCurrency = addEditCurrency.Currency;

        Log.Information("Attempting to inject the new currency symbole \"{NewCurrencySymbole}\"", newCurrency.Symbol);
        var (success, exception) = newCurrency.AddOrEdit();
        if (success)
        {
            Currencies.AddAndSort(newCurrency, s => s.Symbol!);
            Log.Information("Currency symbol was successfully added");
            MsgBox.Show(CurrencyManagementPageResources.MessageBoxAddCurrencySuccess, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(CurrencyManagementPageResources.MessageBoxAddCurrencyError, MsgBoxImage.Error);
        }
    }

    private void ButtonEditCurrency_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not TCurrency currencyToEdit) return;

        var addEditCurrency = new AddEditCurrencyWindow();
        addEditCurrency.SetTCurrency(currencyToEdit);

        var result = addEditCurrency.ShowDialog();
        if (result != true) return;

        if (addEditCurrency.CurrencyDeleted) Currencies.Remove(currencyToEdit);
        else
        {
            var editedCurrency = addEditCurrency.Currency;
            Log.Information("Attempting to update currency symbol id:\"{EditeCurrencyId}\", symbol:\"{EditedCurrencySymbol}\"",editedCurrency.Id, editedCurrency.Symbol);
            var (success, exception) = editedCurrency.AddOrEdit();
            if (success)
            {
                Currencies.Remove(currencyToEdit);
                Currencies.AddAndSort(editedCurrency, s => s.Symbol!);
                Log.Information("Currency symbol was successfully edited");
                //TODO work
                // MsgBox.Show(AccountTypeManagementPageResources.MessageBoxEditAccountTypeSuccess, MsgBoxImage.Check);
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                //TODO work
                // MsgBox.Show(AccountTypeManagementPageResources.MessageBoxEditAccountTypeError, MsgBoxImage.Error);
            }
        }
    }
}