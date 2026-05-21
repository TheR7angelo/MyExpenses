using System.Windows;
using MyExpenses.Presentation.ViewModel;

namespace MyExpenses.Wpf.Pages;

/// <summary>
/// Represents the Account Management Page in the application.
/// </summary>
/// <remarks>
/// This class is responsible for managing the Account Management view and its integration
/// with the corresponding view model. The page is intended to display and manage various
/// aspects of account operations.
/// During initialization, it sets up the page's data context using the provided
/// <see cref="AccountManagementViewModel"/> instance.
/// The page also hooks into the Loaded event to execute an asynchronous load operation
/// using the LoadCommand from the assigned view model.
/// </remarks>
/// <example>
/// This class is typically navigated to through commands or methods such as
/// <see cref="Navigator.NavigateTo"/> within the application.
/// </example>
public partial class AccountManagementPage
{
    /// <summary>
    /// Represents the Account Management Page in the WPF application.
    /// </summary>
    /// <remarks>
    /// This page is associated with the <see cref="AccountManagementViewModel"/> and serves
    /// as the user interface for managing accounts in the application. It initializes
    /// its DataContext to the provided view model instance and monitors the <see cref="FrameworkElement.Loaded"/>
    /// event to trigger the asynchronous execution of the load operation.
    /// </remarks>
    public AccountManagementPage(AccountManagementViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
    }
}