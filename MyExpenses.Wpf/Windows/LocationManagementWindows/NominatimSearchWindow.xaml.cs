using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Application.Interfaces;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Presentation.ViewModels.Locations;

namespace MyExpenses.Wpf.Windows.LocationManagementWindows;

/// <summary> Represents a window for performing Nominatim searches and managing locations. </summary>
public partial class NominatimSearchWindow : IClosable
{
    /// <summary>
    /// Gets or sets the current search result.
    /// </summary>
    private NominatimManagementViewModel ViewModel => (NominatimManagementViewModel)DataContext;

    /// <summary>
    /// Gets or sets the current search result.
    /// </summary>
    public NominatimSearchResultViewModel CurrentSearchResult => ViewModel.CurrentSearchResult;

    /// <summary>
    /// Represents a window for performing Nominatim searches and managing locations.
    /// </summary>
    public NominatimSearchWindow(NominatimManagementViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    /// <summary>
    /// Handles the ContextMenuOpening event for the MapControl, notifying the ViewModel of the current mouse position.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments containing information about the event.</param>
    private void MapControl_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        var position = Mouse.GetPosition(MapControl);
        ViewModel.OnPositionChanged((position.X, position.Y), MapControl);
    }

    /// <summary>
    /// Loads the nominatim search results into the ViewModel.
    /// </summary>
    /// <param name="nominatimSearchResultViewModels">The collection of Nominatim search result view models to load.</param>
    public void LoadNominatimSearchResults(IEnumerable<NominatimSearchResultViewModel> nominatimSearchResultViewModels)
        => ViewModel.LoadNominatimSearchResults(nominatimSearchResultViewModels);
}