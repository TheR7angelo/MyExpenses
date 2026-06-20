using MyExpenses.Presentation.ViewModel;

namespace MyExpenses.Wpf.Pages;

/// <summary>
/// Represents the dashboard page of the application.
/// This class establishes interaction between the UI and the associated view model
/// to display and manage the dashboard's functionality.
/// </summary>
public partial class DashBoardPage
{
    /// <summary>
    /// Represents the dashboard page of the application, providing data binding
    /// and interaction logic with the associated view model.
    /// </summary>
    public DashBoardPage(DashBoardViewModel dashBoardViewModel)
    {
        InitializeComponent();

        DataContext = dashBoardViewModel;
    }
}