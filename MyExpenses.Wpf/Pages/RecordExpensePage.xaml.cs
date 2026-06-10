using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Config;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Utils;

namespace MyExpenses.Wpf.Pages;

public partial class RecordExpensePage : IReceiveNavigationParameter
{
    private ExpenseManagementViewModel ExpenseManagementViewModel
        => (ExpenseManagementViewModel)DataContext;

    public RecordExpensePage(ExpenseManagementViewModel vm)
    {
        InitializeComponent();

        UpdateConfiguration();

        // ReSharper disable once HeapView.DelegateAllocation
        Configuration.ConfigurationChanged += Configuration_OnConfigurationChanged;

        DataContext = vm;
    }

    private void Configuration_OnConfigurationChanged()
        => UpdateConfiguration();

    #region Function

    private void UpdateConfiguration()
    {
        var configuration = Config.Configuration;
        TimePicker.Is24Hours = configuration.Interface.Clock.Is24Hours;

        var backColor = Utils.Resources.GetMaterialDesignPaperMapsUiStylesColor();
        MapControl.Map.BackColor = backColor;
    }

    #endregion

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