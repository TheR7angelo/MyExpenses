using MyExpenses.Presentation.ViewModel;

namespace MyExpenses.Wpf.Pages;

/// <summary>
/// Represents the page for managing bank transfers in the application.
/// </summary>
/// <remarks>
/// This page is associated with the <see cref="BankTransferManagementViewModel"/> and is designed
/// to provide a user interface for performing bank transfer operations. The DataContext of the page
/// is set to the specified view model upon initialization. The associated view model handles the
/// loading logic and supports commands related to bank transfers.
/// </remarks>
/// <example>
/// Usage involves navigating to this page within the application, typically through predefined
/// navigation mechanisms such as buttons or programmatic navigation calls.
/// </example>
public partial class BankTransferPage
{
    /// <summary>
    /// Represents the page for handling bank transfer functionalities within the application.
    /// </summary>
    /// <remarks>
    /// This page binds its visual components to a corresponding instance of
    /// <see cref="BankTransferManagementViewModel"/> to provide a user interface for bank transfer-related
    /// operations. Upon initialization, the view model becomes the DataContext of the page, and its
    /// <c>LoadCommand</c> is executed asynchronously to initialize any necessary data for the page.
    /// </remarks>
    public BankTransferPage(BankTransferManagementViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
    }
}