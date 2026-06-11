using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Wpf.Pages;

/// <summary> Represents a page for recording expenses. </summary>
public partial class RecordExpensePage : IReceiveNavigationParameter
{
    private ExpenseManagementViewModel ExpenseManagementViewModel
        => (ExpenseManagementViewModel)DataContext;

    /// <summary>
    /// Represents a page for recording expenses.
    /// </summary>
    public RecordExpensePage(ExpenseManagementViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
    }

    /// <summary>
    /// Handles the context menu opening event for the MapControl.
    /// This method updates the location information when the context menu is opened on the map.
    /// </summary>
    /// <param name="sender">The source of the event, which is typically the MapControl.</param>
    /// <param name="e">Arguments associated with the event.</param>
    private void MapControl_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        var position = Mouse.GetPosition(MapControl);
        ExpenseManagementViewModel.LocationManagementViewModel.OnPositionChanged(position.X, position.Y, MapControl, true);
    }

    /// <summary>
    /// Receives navigation parameters from the NavigationService.
    /// This is called when the page receives a navigation parameter.
    /// </summary>
    /// <param name="parameter">The parameter passed during navigation</param>
    public void OnNavigationParameterReceived(object? parameter)
    {
        if (parameter is not HistoryViewModel historyViewModel) return;

        ExpenseManagementViewModel.Load(historyViewModel);
    }
}