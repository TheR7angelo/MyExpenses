using MyExpenses.Presentation.ViewModel;

namespace MyExpenses.Wpf.Pages;

/// <summary>
/// Represents the Currency Management page in the application.
/// This page is responsible for the user interface related to managing currencies.
/// It interacts with the <see cref="CurrencyManagementViewModel"/> for data binding and executing commands.
/// </summary>
public partial class CurrencyManagementPage
{
    /// <summary>
    /// Represents the Currency Management page in the application.
    /// This page is responsible for the user interface related to managing currencies.
    /// It interacts with the <see cref="CurrencyManagementViewModel"/> for data binding
    /// and executing commands.
    /// </summary>
    public CurrencyManagementPage(CurrencyManagementViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadCommand.ExecuteAsync(null);
    }
}