using CommunityToolkit.Mvvm.ComponentModel;
using MyExpenses.Presentation.ViewModels.Locations;

namespace MyExpenses.Presentation.ViewModel;

public partial class NominatimManagementViewModel : ViewModelBase
{
    private NominatimSearchResultViewModel[] _searchResults = [];

    private int NominatimSearchResultViewModelCount => _searchResults.Length;

    [ObservableProperty]
    public partial NominatimSearchResultViewModel CurrentSearchResult { get; private set; } = null!;

    [ObservableProperty]
    public partial int CurrentIndex { get; private set; }

    [ObservableProperty]
    public partial string WindowTitle { get; private set; } = string.Empty;

    public void LoadNominatimSearchResults(IEnumerable<NominatimSearchResultViewModel> searchResults)
    {
        _searchResults = searchResults.ToArray();
        CurrentSearchResult = _searchResults[CurrentIndex];
        WindowTitle = $"{CurrentIndex+1}/{NominatimSearchResultViewModelCount} - {CurrentSearchResult.DisplayName}";
    }
}