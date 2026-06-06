namespace MyExpenses.Presentation.ViewModel;


public partial class ExpenseManagementViewModel : ViewModelBase
{
    public LocationManagementViewModel LocationManagementViewModel { get; private set; }

    public ExpenseManagementViewModel(LocationManagementViewModel locationManagementViewModel)
    {
        LocationManagementViewModel = locationManagementViewModel;
    }
}