using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Services.Interfaces;

/// <summary>
/// Provides navigation services for displaying various account-related windows in the application.
/// This interface defines methods for launching windows or dialogs related to adding, editing,
/// and managing accounts or account types.
/// </summary>
public interface INavigationWindowService
{
    /// <summary>
    /// Displays the "Manage Account" window as a dialog.
    /// This method is used to navigate to the interface where users can view or manage the details of an account.
    /// </summary>
    /// <param name="item">The view model representing the account to be managed. This contains the account's current details or configuration.</param>
    public Task ShowManageAccount(TotalByAccountViewModel? item);

    /// <summary>
    /// Displays the "Edit Account" window as a dialog.
    /// This method is used to navigate to the interface where users can edit an existing account's details.
    /// </summary>
    /// <param name="vm">The view model representing the account to be edited. This contains the account's current data.</param>
    public void ShowManageAccount(AccountViewModel? vm);

    /// <summary>
    /// Displays the "Add Account Type" window as a dialog.
    /// This method is used to navigate to the interface where users can add a new account type.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ShowAddAccountType();

    /// <summary>
    /// Displays the "Edit Account Type" view as a dialog asynchronously.
    /// This method allows editing of the properties of a specific account type.
    /// </summary>
    /// <param name="item">The account type view model representing the account type to be edited.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ShowEditAccountTypeAsync(AccountTypeViewModel item);

    /// <summary>
    /// Displays the "Manage Category Type" window.
    /// This method allows users to manage the details of a specific category type, such as its name,
    /// color, and other metadata. It is commonly used for creating, editing, or reviewing category types.
    /// </summary>
    /// <param name="categoryTypeViewModel">An instance of <see cref="CategoryTypeViewModel"/> containing
    /// the category type data to be managed. If null, a new category type may be created.</param>
    public void ShowManageCategoryType(CategoryTypeViewModel? categoryTypeViewModel);

    /// <summary>
    /// Displays the "Color Management" window as a dialog.
    /// This method is used to navigate to the interface where users can view, modify, or manage color-related settings or details.
    /// </summary>
    /// <param name="color">The view model representing the color to be managed. This contains details such as the color name and hexadecimal code.</param>
    public void ShowColorManagementWindow(ColorViewModel? color);

    /// <summary>
    /// Opens the official GitHub page associated with the application.
    /// This method is typically used to navigate users to the repository or documentation hosted on GitHub for further information or assistance.
    /// </summary>
    public void OpenGithubPage();

    /// <summary>
    /// Opens a specified URI in the default web browser or associated application.
    /// This method is used to navigate to external resources or links from within the application.
    /// </summary>
    /// <param name="uri">The URI to be opened. This should be a properly formatted string representing a valid URI.</param>
    public void OpenUri(string uri);
}