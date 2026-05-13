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

    private void ButtonValidBankTransferPrepare_OnClick(object sender, RoutedEventArgs e)
    {
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // The serviceProvider and items are set to null because they are not required in this context.
        // // The ValidationResults list will store any validation errors detected during the process.
        // var validationContext = new ValidationContext(BankTransfer, serviceProvider: null, items: null);
        //
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // Using 'var' keeps the code concise and readable, as the type (List<ValidationResult>)
        // // is evident from the initialization. The result will still be compatible with any method
        // // that expects an ICollection<ValidationResult>, as List<T> implements the ICollection interface.
        // var validationResults = new List<ValidationResult>();
        // var isValid = Validator.TryValidateObject(BankTransfer, validationContext, validationResults, true);
        //
        // if (!isValid)
        // {
        //     var propertyError = validationResults.First();
        //     var propertyMemberName = propertyError.MemberNames.First();
        //
        //     var messageErrorKey = propertyMemberName switch
        //     {
        //         nameof(TBankTransfer.FromAccountFk) => nameof(BankTransferManagementResources.MessageBoxButtonValidationFromAccountFkError),
        //         nameof(TBankTransfer.ToAccountFk) => nameof(BankTransferManagementResources.MessageBoxButtonValidationToAccountFkError),
        //         nameof(TBankTransfer.Value) => nameof(BankTransferManagementResources.MessageBoxButtonValidationValueError),
        //         nameof(TBankTransfer.Date) => nameof(BankTransferManagementResources.MessageBoxButtonValidationDateError),
        //         nameof(TBankTransfer.MainReason) => nameof(BankTransferManagementResources.MessageBoxButtonValidationMainReasonError),
        //         _ => null
        //     };
        //
        //     var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
        //         ? propertyError.ErrorMessage!
        //         : BankTransferManagementResources.ResourceManager.GetString(messageErrorKey)!;
        //
        //     MsgBox.Show(localizedErrorMessage, MsgBoxImage.Error);
        //     return;
        // }
        //
        // if (Category is null)
        // {
        //     MsgBox.Show(BankTransferManagementResources.MessageBoxButtonValidBankTransferPrepareCategoryIsNullError,
        //         MsgBoxImage.Warning);
        //     return;
        // }
        //
        // if (ModePayment is null)
        // {
        //     MsgBox.Show(BankTransferManagementResources.MessageBoxButtonValidBankTransferPrepareModePaymentIsNullError,
        //         MsgBoxImage.Warning);
        //     return;
        // }
        //
        // BankTransferPrepare = true;
    }

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