using MyExpenses.Presentation.ViewModel;

namespace MyExpenses.Wpf.Pages;

/// <summary>
/// Represents a view for managing payment modes.
/// </summary>
public partial class ModePaymentManagementPage
{
    /// <summary>
    /// Represents a view for managing payment modes.
    /// </summary>
    public ModePaymentManagementPage(ModePaymentManagementViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}