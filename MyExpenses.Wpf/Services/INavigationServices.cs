using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Wpf.Services;

public interface INavigationServices
{
    /// <summary>
    /// Displays the "Add Account" window as a dialog.
    /// This method is used to navigate to the interface where users can add a new account.
    /// </summary>
    public void ShowAddAccount();

    /// <summary>
    /// Displays the "Edit Account" window as a dialog.
    /// This method allows users to edit the details of an existing account.
    /// </summary>
    /// <param name="vm">The view model containing the account details to be edited.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ShowEditAccountAsync(TotalByAccountViewModel vm);
}