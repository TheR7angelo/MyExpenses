using MyExpenses.Presentation.ViewModel;

namespace MyExpenses.Wpf.Pages;

public partial class AccountTypeManagementPage
{
    // public ObservableCollection<TAccountType> AccountTypes { get; } = [];

    public AccountTypeManagementPage(AccountTypeManagementViewModel vm)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        // using var context = new DataBaseContextOld();
        // AccountTypes = [..context.TAccountTypes.OrderBy(s => s.Name)];

        InitializeComponent();

        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadCommand.ExecuteAsync(null);
    }

    // private void ButtonAddNewAccountType_OnClick(object sender, RoutedEventArgs e)
    // {
    //     // ReSharper disable once HeapView.ObjectAllocation.Evident
    //     // The instance of AddEditAccountTypeWindow is created locally within this method and is used temporarily.
    //     // Since there are no references to it after this scope and the Garbage Collector will handle
    //     // its cleanup efficiently, this allocation is intentional and does not require further optimization.
    //
    //     var addEditAccountType = App.ServiceProvider.GetRequiredService<AddEditAccountTypeWindow>();
    //     var result = addEditAccountType.ShowDialog();
    //     if (result is not true) return;
    //
    //     var newAccountType = addEditAccountType.AccountType;
    //
    //     Log.Information("Attempting to inject the new account type \"{NewAccountTypeName}\"", newAccountType.Name);
    //     // TODO Inject
    //     // var (success, exception) = newAccountType.AddOrEdit();
    //     // if (success)
    //     // {
    //     //     AccountTypes.AddAndSort(newAccountType, s => s.Name!);
    //     //
    //     //     Log.Information("Account type was successfully added");
    //     //     var json = newAccountType.ToJsonString();
    //     //     Log.Information("{Json}", json);
    //     //
    //     //     MsgBox.Show(AccountTypeManagementResources.MessageBoxAddNewAccountTypeSuccessTitle,
    //     //         AccountTypeManagementResources.MessageBoxAddNewAccountTypeSuccessMessage, MsgBoxImage.Check);
    //     // }
    //     // else
    //     // {
    //     //     Log.Error(exception, "An error occurred please retry");
    //     //     MsgBox.Show(AccountTypeManagementResources.MessageBoxAddNewAccountTypeErrorTitle,
    //     //         AccountTypeManagementResources.MessageBoxAddNewAccountTypeErrorMessage, MsgBoxImage.Error);
    //     // }
    // }

    // private void ButtonAccountType_OnClick(object sender, RoutedEventArgs e)
    // {
    //     var button = (Button)sender;
    //     if (button.DataContext is not TAccountType accountType) return;
    //
    //     // ReSharper disable once HeapView.ObjectAllocation.Evident
    //     // The instance of AddEditAccountTypeWindow is created locally within this method and is used temporarily.
    //     // Since there are no references to it after this scope and the Garbage Collector will handle
    //     // its cleanup efficiently, this allocation is intentional and does not require further optimization.
    //     var addEditAccountType = App.ServiceProvider.GetRequiredService<AddEditAccountTypeWindow>();
    //     addEditAccountType.SetAccountType(accountType);
    //
    //     var result = addEditAccountType.ShowDialog();
    //     if (result is not true) return;
    //
    //     var editedAccountType = addEditAccountType.AccountType;
    //     if (addEditAccountType.AccountTypeDeleted) AccountTypes.Remove(accountType);
    //     else
    //     {
    //         Log.Information("Attempting to update account type id:\"{EditedAccountTypeId}\", name:\"{EditedAccountTypeName}\"",editedAccountType.Id, editedAccountType.Name);
    //         // TODO Inject
    //         // var (success, exception) = editedAccountType.AddOrEdit();
    //         // if (success)
    //         // {
    //         //     AccountTypes.Remove(accountType);
    //         //     AccountTypes.AddAndSort(editedAccountType, s => s.Name!);
    //         //
    //         //     Log.Information("Account type was successfully edited");
    //         //     var json = editedAccountType.ToJsonString();
    //         //     Log.Information("{Json}", json);
    //         //
    //         //     MsgBox.Show(AccountTypeManagementResources.MessageBoxAccountTypeEditSuccessTitle,
    //         //         AccountTypeManagementResources.MessageBoxAccountTypeEditSuccessMessage, MsgBoxImage.Check);
    //         // }
    //         // else
    //         // {
    //         //     Log.Error(exception, "An error occurred please retry");
    //         //     MsgBox.Show(AccountTypeManagementResources.MessageBoxAccountTypeEditErrorTitle,
    //         //     AccountTypeManagementResources.MessageBoxAccountTypeEditErrorMessage, MsgBoxImage.Error);
    //         // }
    //     }
    // }
}