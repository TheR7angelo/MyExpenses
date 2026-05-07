using MyExpenses.Presentation.ViewModel;

namespace MyExpenses.Wpf.Pages;

/// <summary>
/// Represents the account type management page that displays and manages the collection of account types.
/// </summary>
public partial class AccountTypeManagementPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountTypeManagementPage"/> class.
    /// </summary>
    /// <param name="vm">The <see cref="AccountTypeManagementViewModel"/> that provides the data and commands for this page.</param>
    public AccountTypeManagementPage(AccountTypeManagementViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;

        Loaded += async (_, _) => await vm.LoadCommand.ExecuteAsync(null);
    }
}