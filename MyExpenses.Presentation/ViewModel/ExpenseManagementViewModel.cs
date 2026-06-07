using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Locations;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Utils.Converters;

namespace MyExpenses.Presentation.ViewModel;


public partial class ExpenseManagementViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial HistoryViewModel HistoryViewModel { get; private set; } = new();

    public LocationManagementViewModel LocationManagementViewModel { get; }

    public ObservableCollection<AccountViewModel> Accounts { get; } = [];
    public ObservableCollection<CategoryTypeViewModel> CategoryTypes { get; } = [];
    public ObservableCollection<ModePaymentViewModel> ModePaymentViewModels { get; } = [];
    private ObservableCollection<PlaceViewModel> Places { get; } = [];

    public ObservableCollection<string> AvailableCountries { get; } = [];
    public ObservableCollection<string> AvailableCities { get; } = [];
    public ObservableCollection<PlaceViewModel> FilteredPlaces { get; } = [];

    [ObservableProperty]
    public partial string? SelectedCountry { get; set; }

    [ObservableProperty]
    public partial string? SelectedCity { get; set; }

    [ObservableProperty]
    public partial PlaceViewModel? SelectedPlace { get; set; }

    [ObservableProperty]
    public partial bool IsHistoryEdit { get; set; }

    public static string TextSearchLocationName { get; } = nameof(PlaceViewModel.Name);

    private readonly IAccountPresentationService _accountPresentationService;
    private readonly IExpensePresentationService _expensePresentationService;
    private readonly ILocationPresentationService _locationPresentationService;
    private readonly INavigationWindowService _navigationWindowService;
    private readonly IAccountDtoViewModelMapper _accountDtoViewModelMapper;

    private readonly ILogger<ExpenseManagementViewModel> _logger;

    public ExpenseManagementViewModel(LocationManagementViewModel locationManagementViewModel,
        IAccountPresentationService accountPresentationService, IExpensePresentationService expensePresentationService,
        ILocationPresentationService locationPresentationService,
        INavigationWindowService navigationWindowService,
        IAccountDtoViewModelMapper accountDtoViewModelMapper,
        ILogger<ExpenseManagementViewModel> logger)
    {
        LocationManagementViewModel = locationManagementViewModel;
        _accountPresentationService = accountPresentationService;
        _expensePresentationService = expensePresentationService;
        _locationPresentationService = locationPresentationService;
        _navigationWindowService = navigationWindowService;
        _accountDtoViewModelMapper = accountDtoViewModelMapper;

        _logger = logger;

        Places.CollectionChanged += (_, _) =>
        {
            UpdateAvailableCountries();
            UpdateFilteredPlaces();
        };

        HistoryViewModel?.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(HistoryViewModel.PlaceViewModel))
            {
                UpdateFiltersFromHistoryPlace(HistoryViewModel.PlaceViewModel);
            }
        };

        RegisterMessages();
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, OnItemDeleted);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountViewModel>>(this, OnAccountChanged);
    }

    private void OnAccountChanged(object recipient, EntityChangedMessage<AccountViewModel> m)
    {
        if (m.Value.EntityType is not DependencyType.Account) return;

        switch (m.Value.DataAction)
        {
            case DataAction.Update:
                ApplyUpdate(m.Value.Content);
                break;

            case DataAction.Add:
                ApplyAddAsync(m.Value.Content);
                break;
        }
    }

    private void ApplyUpdate(AccountViewModel vm)
    {
        var item = Accounts.FirstOrDefault(s => s.Id == vm.Id);
        if (item is null) return;

        _accountDtoViewModelMapper.Merge(vm, item);
    }

    private void ApplyAddAsync(AccountViewModel vm)
    {
        Accounts.AddAndSort(vm, s => s.Name!);
        HistoryViewModel.AccountViewModel = vm;
    }

    private void OnItemDeleted(object recipient, EntityChangedMessage<int[]> m)
    {
        if (m.Value.DataAction is not DataAction.Delete) return;

        if (m.Value.EntityType is DependencyType.Account) OnAccountDeleted(m.Value.Content);
    }

    private void OnAccountDeleted(int[] m)
    {
        var toDeletes = Accounts.Where(s => m.Contains(s.Id)).ToList();
        Accounts.RemoveRange(toDeletes);
    }

    [RelayCommand]
    private void OnManageAccount()
        => _navigationWindowService.ShowManageAccount(HistoryViewModel.AccountViewModel);

    [RelayCommand]
    private async Task OnLoad(CancellationToken cancellationToken = default)
    {
        LocationManagementViewModel.LoadCommand.Execute(null);

        var accountsTask = _accountPresentationService.GetAllAccountViewModelAsync(cancellationToken);
        var categoryTypesTask = _expensePresentationService.GetAllCategoryTypeViewModelAsync(cancellationToken);
        var modePaymentsTask = _expensePresentationService.GetAllModePaymentViewModelAsync(cancellationToken);
        var locationsTask = _locationPresentationService.GetAllPlaces(cancellationToken);

        await Task.WhenAll(accountsTask, categoryTypesTask, modePaymentsTask, locationsTask);

        Accounts.AddRangeAndSort(accountsTask, x => x.Name!, logger: _logger);
        CategoryTypes.AddRangeAndSort(categoryTypesTask, x => x.Name!, logger: _logger);
        ModePaymentViewModels.AddRangeAndSort(modePaymentsTask, x => x.Name!, logger: _logger);
        Places.AddRangeAndSort(locationsTask, x => x.Name!, logger: _logger);


        UpdateAvailableCountries();
    }

    private void UpdateAvailableCountries()
    {
        var countries = Places
            .Select(p => EmptyStringTreeViewConverter.ToUnknown(p.Country))
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        AvailableCountries.Clear();
        AvailableCountries.AddRange(countries);
    }

    private void UpdateAvailableCities()
    {
        AvailableCities.Clear();
        SelectedCity = null;

        if (string.IsNullOrEmpty(SelectedCountry)) return;

        var cities = Places
            .Where(p => EmptyStringTreeViewConverter.ToUnknown(p.Country) == SelectedCountry)
            .Select(p => EmptyStringTreeViewConverter.ToUnknown(p.City))
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        AvailableCities.AddRange(cities);
    }

    private void UpdateFilteredPlaces()
    {
        FilteredPlaces.Clear();

        var filtered = Places.AsEnumerable();

        if (!string.IsNullOrEmpty(SelectedCountry))
        {
            filtered = filtered.Where(p => EmptyStringTreeViewConverter.ToUnknown(p.Country) == SelectedCountry);
        }

        if (!string.IsNullOrEmpty(SelectedCity))
        {
            filtered = filtered.Where(p => EmptyStringTreeViewConverter.ToUnknown(p.City) == SelectedCity);
        }

        var places = filtered
            .OrderBy(p => p.Name)
            .ToList();

        FilteredPlaces.AddRange(places);
    }

    partial void OnSelectedCountryChanged(string? oldValue, string? newValue)
    {
        UpdateAvailableCities();
        UpdateFilteredPlaces();
    }

    partial void OnSelectedCityChanged(string? oldValue, string? newValue)
    {
        UpdateFilteredPlaces();
    }

    partial void OnSelectedPlaceChanged(PlaceViewModel? oldValue, PlaceViewModel? newValue)
    {
        if (newValue is null) return;

        SelectedCountry = EmptyStringTreeViewConverter.ToUnknown(newValue.Country);
        SelectedCity = EmptyStringTreeViewConverter.ToUnknown(newValue.City);
    }

    private void UpdateFiltersFromHistoryPlace(PlaceViewModel? placeViewModel)
    {
        if (placeViewModel is null)
        {
            SelectedCountry = null;
            SelectedCity = null;
            SelectedPlace = null;
            return;
        }

        SelectedCountry = EmptyStringTreeViewConverter.ToUnknown(placeViewModel.Country);
        SelectedCity = EmptyStringTreeViewConverter.ToUnknown(placeViewModel.City);
        SelectedPlace = placeViewModel;
    }
}