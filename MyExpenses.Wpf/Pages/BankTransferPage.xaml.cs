using System.Windows;
using MyExpenses.Presentation.ViewModel;

namespace MyExpenses.Wpf.Pages;

public partial class BankTransferPage
{
    public BankTransferPage(BankTransferManagementViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadCommand.ExecuteAsync(null);
    }

    #region Action

    private void ButtonValidBankTransferPreview_OnClick(object sender, RoutedEventArgs e)
    {
        // var now = DateTime.Now;
        // var valueAbs = Math.Abs(BankTransfer.Value ?? 0);
        //
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // The THistories collection is created here to store the THistory instances for the transfer.
        // var fromHistory = new THistory
        // {
        //     AccountFk = BankTransfer.FromAccountFk,
        //     Description = BankTransfer.MainReason,
        //     CategoryTypeFk = Category?.Id,
        //     ModePaymentFk = ModePayment?.Id,
        //     Value = -valueAbs,
        //     Date = BankTransfer.Date,
        //     IsPointed = true,
        //     PlaceFk = 1,
        //     DateAdded = now,
        //     DatePointed = now
        // };
        //
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // The THistories collection is created here to store the THistory instances for the transfer.
        // var toHistory = new THistory
        // {
        //     AccountFk = BankTransfer.ToAccountFk,
        //     Description = BankTransfer.MainReason,
        //     CategoryTypeFk = Category?.Id,
        //     ModePaymentFk = ModePayment?.Id,
        //     Value = valueAbs,
        //     Date = BankTransfer.Date,
        //     IsPointed = true,
        //     PlaceFk = 1,
        //     DateAdded = now,
        //     DatePointed = now
        // };
        //
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // Ensuring that the THistories collection is initialized with an empty List<THistory>
        // // if it is null. This prevents potential NullReferenceExceptions when attempting
        // // to add items to the collection. It ensures that the property is always in a valid
        // // state and ready for use regardless of its initial state.
        // BankTransfer.THistories ??= new List<THistory>();
        // BankTransfer.THistories.Add(fromHistory);
        // BankTransfer.THistories.Add(toHistory);
        //
        // var (success, exception) = BankTransfer.AddOrEdit();
        // if (success)
        // {
        //     Log.Information(
        //         "The transfer has been successfully completed, {FromName} to {ToName} with value {ValueAbs}",
        //         VFromAccount!.Name, VToAccount!.Name, valueAbs);
        //
        //     // Loop crash
        //     // var json = BankTransfer.ToJsonString();
        //     // Log.Information("{Json}", json);
        //
        //     MsgBox.Show(BankTransferManagementResources.MessageBoxButtonValidBankTransferPreviewSuccess, MsgBoxImage.Check);
        //
        //     var response = MsgBox.Show(
        //         BankTransferManagementResources.MessageBoxButtonValidBankTransferPreviewNewTransferQuestion,
        //         MsgBoxImage.Question, MessageBoxButton.YesNo);
        //
        //     if (response is not MessageBoxResult.Yes) nameof(MainWindow.FrameBody).GoBack();
        //     else
        //     {
        //         BankTransfer.Reset();
        //         BankTransferPrepare = false;
        //     }
        // }
        // else
        // {
        //     Log.Error(exception, "An error occurred please retry");
        //     MsgBox.Show(BankTransferManagementResources.MessageBoxButtonValidBankTransferPreviewError, MsgBoxImage.Error);
        // }
    }

    private void ButtonFromAddAccount_OnClick(object sender, RoutedEventArgs e)
    {
        // var addEditAccountWindow  = App.ServiceProvider.GetRequiredService<AddEditAccountWindow>();
        //
        // var fromAccount = BankTransfer.FromAccountFk.ToISql<TAccount>();
        // if (fromAccount is not null) addEditAccountWindow.SetTAccount(fromAccount);
        //
        // addEditAccountWindow.ShowDialog();
        // if (addEditAccountWindow.DialogResult is not true) return;
        //
        // if (addEditAccountWindow.DeleteAccount)
        // {
        //     RemoveByAccountId(BankTransfer.FromAccountFk);
        //     return;
        // }
        //
        // var editedAccount = addEditAccountWindow.Account;
        //
        // Log.Information("Attempting to edit the account \"{AccountName}\"", editedAccount.Name);
        // var (success, exception) = editedAccount.AddOrEdit();
        // if (success)
        // {
        //     Log.Information("Account was successfully edited");
        //     var json = editedAccount.ToJsonString();
        //     Log.Information("{Json}", json);
        //
        //     MsgBox.Show(BankTransferManagementResources.MessageBoxEditAccountSuccess, MsgBoxImage.Check);
        //
        //     RemoveByAccountId(editedAccount.Id);
        //     Accounts.AddAndSort(editedAccount, s => s.Name!);
        //     FromAccounts.AddAndSort(editedAccount, s => s.Name!);
        //     BankTransfer.FromAccountFk = editedAccount.Id;
        // }
        // else
        // {
        //     Log.Error(exception, "An error occurred please retry");
        //     MsgBox.Show(BankTransferManagementResources.MessageBoxEditAccountError, MsgBoxImage.Warning);
        // }
    }

    private void ButtonToAddAccount_OnClick(object sender, RoutedEventArgs e)
    {
        // var addEditAccountWindow  = App.ServiceProvider.GetRequiredService<AddEditAccountWindow>();
        //
        // var fromAccount = BankTransfer.ToAccountFk.ToISql<TAccount>();
        // if (fromAccount is not null) addEditAccountWindow.SetTAccount(fromAccount);
        //
        // addEditAccountWindow.ShowDialog();
        // if (addEditAccountWindow.DialogResult is not true) return;
        //
        // if (addEditAccountWindow.DeleteAccount)
        // {
        //     RemoveByAccountId(BankTransfer.ToAccountFk);
        //     return;
        // }
        //
        // var editedAccount = addEditAccountWindow.Account;
        //
        // Log.Information("Attempting to edit the account \"{AccountName}\"", editedAccount.Name);
        // var (success, exception) = editedAccount.AddOrEdit();
        // if (success)
        // {
        //     Log.Information("Account was successfully edited");
        //     var json = editedAccount.ToJsonString();
        //     Log.Information("{Json}", json);
        //
        //     MsgBox.Show(BankTransferManagementResources.MessageBoxEditAccountSuccess, MsgBoxImage.Check);
        //
        //     RemoveByAccountId(editedAccount.Id);
        //     Accounts.AddAndSort(editedAccount, s => s.Name!);
        //     ToAccounts.AddAndSort(editedAccount, s => s.Name!);
        //
        //     BankTransfer.ToAccountFk = editedAccount.Id;
        // }
        // else
        // {
        //     Log.Error(exception, "An error occurred please retry");
        //     MsgBox.Show(BankTransferManagementResources.MessageBoxEditAccountError, MsgBoxImage.Warning);
        // }
    }

    #endregion
}