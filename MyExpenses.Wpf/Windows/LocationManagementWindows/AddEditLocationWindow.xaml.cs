using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Application.Interfaces;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Presentation.ViewModels.Locations;

namespace MyExpenses.Wpf.Windows.LocationManagementWindows;

/// <summary>
/// Window used to add or edit a location. It exposes a
/// <see cref="LocationManagementViewModel"/> as its DataContext and
/// delegates location-related operations to that view model.
/// </summary>
public partial class AddEditLocationWindow : IClosable
{
    private LocationManagementViewModel ViewModel => (LocationManagementViewModel)DataContext;

    /// <summary>
    /// Initializes a new instance of <see cref="AddEditLocationWindow"/>.
    /// </summary>
    /// <param name="viewModel">The <see cref="LocationManagementViewModel"/> used as the window DataContext.</param>
    public AddEditLocationWindow(LocationManagementViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    /// <summary>
    /// Loads a <see cref="PlaceViewModel"/> into the associated view model and indicates
    /// whether the operation is an edit or a creation. The call is delegated to the
    /// <see cref="LocationManagementViewModel"/>.
    /// </summary>
    /// <param name="placeViewModel">The place view model to load.</param>
    /// <param name="isEdit">If true, the window is in edit mode; otherwise it is in create mode.</param>
    public void LoadPlaceViewModel(PlaceViewModel placeViewModel, bool isEdit)
        => ViewModel.LoadPlaceViewModel(placeViewModel, isEdit);

    /// <summary>
    /// Handles the context menu opening event for the map control.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    private void MapControl_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        var position = Mouse.GetPosition(MapControl);
        ViewModel.OnPositionChanged(position.X, position.Y, MapControl, false);
    }
}