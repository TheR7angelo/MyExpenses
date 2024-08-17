using System.Collections.ObjectModel;
using System.Windows;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Resources.Resx.Pages.RecordExpensePage;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.CategoryTypeManagementWindow;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditRecurrentExpenseWindow
{
    public ObservableCollection<TAccount> Accounts { get; }
    public ObservableCollection<TCategoryType> CategoryTypes { get; }

    public string SelectedValuePathAccount { get; } = nameof(TAccount.Id);
    public string DisplayMemberPathAccount { get; } = nameof(TAccount.Name);
    public string SelectedValuePathCategoryType { get; } = nameof(TCategoryType.Id);
    public string DisplayMemberPathCategoryType { get; } = nameof(TCategoryType.Name);

    public TRecursiveExpense RecursiveExpense { get; set; } = new();
    public AddEditRecurrentExpenseWindow()
    {
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];
        CategoryTypes = [..context.TCategoryTypes.OrderBy(s => s.Name)];

        InitializeComponent();

        this.SetWindowCornerPreference();
    }

    public void SetVRecursiveExpense(VRecursiveExpense vRecurrentExpense)
    {
        vRecurrentExpense.CopyPropertiesTo(RecursiveExpense);
    }

    private void ButtonAccount_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditAccountWindow = new AddEditAccountWindow();

        var account = RecursiveExpense.AccountFk?.ToISql<TAccount>();
        if (account is not null) addEditAccountWindow.SetTAccount(account);

        addEditAccountWindow.ShowDialog();
        if (addEditAccountWindow.DialogResult is not true) return;

        if (addEditAccountWindow.DeleteAccount)
        {
            var accountToRemove = Accounts.FirstOrDefault(s => s.Id == RecursiveExpense.AccountFk);
            if (accountToRemove is not null) Accounts.Remove(accountToRemove);
        }
        else
        {
            var editedAccount = addEditAccountWindow.Account;

            Log.Information("Attempting to edit the account \"{AccountName}\"", editedAccount.Name);
            var (success, exception) = editedAccount.AddOrEdit();
            if (success)
            {
                Log.Information("Account was successfully edited");
                var json = editedAccount.ToJsonString();
                Log.Information("{Json}", json);

                MsgBox.MsgBox.Show(RecordExpensePageResources.MessageBoxEditAccountSuccess, MsgBoxImage.Check);

                var accountToRemove = Accounts.FirstOrDefault(s => s.Id == RecursiveExpense.AccountFk);
                Accounts!.AddAndSort(accountToRemove, editedAccount, s => s?.Name!);

                RecursiveExpense.AccountFk = editedAccount.Id;
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                MsgBox.MsgBox.Show(RecordExpensePageResources.MessageBoxEditAccountError, MsgBoxImage.Warning);
            }
        }
    }

    private void ButtonCategoryType_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditCategoryTypeWindow = new AddEditCategoryTypeWindow();
        var categoryType = RecursiveExpense.CategoryTypeFk?.ToISql<TCategoryType>();
        if (categoryType is not null) addEditCategoryTypeWindow.SetTCategoryType(categoryType);

        var result = addEditCategoryTypeWindow.ShowDialog();
        if (result is not true) return;

        if (addEditCategoryTypeWindow.CategoryTypeDeleted)
        {
            var categoryTypeToRemove = CategoryTypes.FirstOrDefault(s => s.Id == RecursiveExpense.CategoryTypeFk);
            if (categoryTypeToRemove is not null) CategoryTypes.Remove(categoryTypeToRemove);
        }
        else
        {
            var editedCategoryType = addEditCategoryTypeWindow.CategoryType;
            Log.Information("Attempting to edit the category type id: {Id}", editedCategoryType.Id);

            var editedCategoryTypeDeepCopy = editedCategoryType.DeepCopy();

            var (success, exception) = editedCategoryType.AddOrEdit();
            if (success)
            {
                using var context = new DataBaseContext();
                editedCategoryTypeDeepCopy.ColorFkNavigation =
                    context.TColors.FirstOrDefault(s => s.Id == editedCategoryTypeDeepCopy.ColorFk);

                CategoryTypes!.AddAndSort(categoryType, editedCategoryTypeDeepCopy, s => s!.Name!);

                Log.Information("Category type was successfully edited");
                var json = editedCategoryTypeDeepCopy.ToJsonString();
                Log.Information("{Json}", json);

                MsgBox.MsgBox.Show(RecordExpensePageResources.MessageBoxEditCategorySuccess, MsgBoxImage.Check);
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                MsgBox.MsgBox.Show(RecordExpensePageResources.MessageBoxEditCategoryError, MsgBoxImage.Error);
            }
        }
    }
}