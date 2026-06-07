using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Domain.Models.Systems;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.ExpenseResources;
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
    public partial bool IsHistoryEdit { get; set; }

    public static string TextSearchLocationName { get; } = nameof(PlaceViewModel.Name);

    private bool _isLoaded;

    private readonly IAccountPresentationService _accountPresentationService;
    private readonly IExpensePresentationService _expensePresentationService;
    private readonly ILocationPresentationService _locationPresentationService;
    private readonly IExpenseActionService _expenseActionService;
    private readonly INavigationWindowService _navigationWindowService;
    private readonly IDialogService _dialogService;
    private readonly IAccountDtoViewModelMapper _accountDtoViewModelMapper;
    private readonly IExpenseDtoViewModelMapper _expenseDtoViewModelMapper;
    private readonly ILocationDtoViewModelMapper _locationDtoViewModelMapper;

    private readonly ILogger<ExpenseManagementViewModel> _logger;

    public ExpenseManagementViewModel(LocationManagementViewModel locationManagementViewModel,
        IAccountPresentationService accountPresentationService, IExpensePresentationService expensePresentationService,
        ILocationPresentationService locationPresentationService,
        IExpenseActionService expenseActionService,
        INavigationWindowService navigationWindowService,
        IDialogService dialogService,
        IAccountDtoViewModelMapper accountDtoViewModelMapper,
        IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
        ILocationDtoViewModelMapper locationDtoViewModelMapper,
        ILogger<ExpenseManagementViewModel> logger)
    {
        LocationManagementViewModel = locationManagementViewModel;
        _accountPresentationService = accountPresentationService;
        _expensePresentationService = expensePresentationService;
        _locationPresentationService = locationPresentationService;
        _expenseActionService = expenseActionService;
        _navigationWindowService = navigationWindowService;
        _dialogService = dialogService;
        _accountDtoViewModelMapper = accountDtoViewModelMapper;
        _expenseDtoViewModelMapper = expenseDtoViewModelMapper;
        _locationDtoViewModelMapper = locationDtoViewModelMapper;

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
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountViewModel>>(this, (_, m) =>
            OnEntityChanged(m, Accounts, DependencyType.Account,
                getId: vm => vm.Id,
                getName: vm => vm.Name,
                merge: (src, target) => _accountDtoViewModelMapper.Merge(src, target),
                update: vm => HistoryViewModel.AccountViewModel = vm));

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<CategoryTypeViewModel>>(this, (_, m) =>
            OnEntityChanged(m, CategoryTypes, DependencyType.CategoryType,
                getId: vm => vm.Id,
                getName: vm => vm.Name,
                merge: (src, target) => _expenseDtoViewModelMapper.Merge(src, target),
                update: vm => HistoryViewModel.CategoryTypeViewModel = vm));

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<ModePaymentViewModel>>(this, (_, m) =>
            OnEntityChanged(m, ModePaymentViewModels, DependencyType.ModePayment,
                getId: vm => vm.Id,
                getName: vm => vm.Name,
                merge: (src, target) => _expenseDtoViewModelMapper.Merge(src, target),
                update: vm => HistoryViewModel.ModePaymentViewModel = vm));

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<PlaceViewModel>>(this, (_, m) =>
            OnEntityChanged(m, Places, DependencyType.Place,
                getId: vm => vm.Id,
                getName: vm => vm.Name,
                merge: (src, target) => _locationDtoViewModelMapper.Merge(src, target),
                update: vm => HistoryViewModel.PlaceViewModel = vm));

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, (_, m) =>
        {
            OnItemDeleted(m, Accounts, DependencyType.Account, x => x.Id);
            OnItemDeleted(m, CategoryTypes, DependencyType.CategoryType, x => x.Id);
            OnItemDeleted(m, ModePaymentViewModels, DependencyType.ModePayment, x => x.Id);
            OnItemDeleted(m, Places, DependencyType.Place, x => x.Id);
        });
    }

    [RelayCommand]
    private void OnManageAccount()
        => _navigationWindowService.ShowManageAccount(HistoryViewModel.AccountViewModel);

    [RelayCommand]
    private void OnManageCategoryType()
        => _navigationWindowService.ShowManageCategoryType(HistoryViewModel.CategoryTypeViewModel);

    [RelayCommand]
    private async Task OnManageModePayment(CancellationToken cancellationToken = default)
    {
        if (HistoryViewModel.ModePaymentViewModel is null || HistoryViewModel.ModePaymentViewModel.CanBeDeleted)
        {
            await _expenseActionService.ManageModePaymentAction(HistoryViewModel.ModePaymentViewModel, cancellationToken);
        }
        else
        {
            _dialogService.ShowMessageBox(ExpenseResources.MessageBoxErrorEditDefaultPaymentMethodCaption,
                ExpenseResources.MessageBoxErrorEditDefaultPaymentMethodContent,
                MessageBoxButton.Ok,
                MsgBoxImage.Warning);
        }
    }

    [RelayCommand]
    private void OnDateNow()
        => HistoryViewModel.Date = DateTime.Now;

    [RelayCommand]
    private void OnManagePlace()
        => _navigationWindowService.ShowLocationManagementWindow(HistoryViewModel.PlaceViewModel, false);

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
        InitializeZoom();

        _isLoaded = true;
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

        if (!_isLoaded) return;
        var points = places.Where(s => s.Id is not PlaceDomain.DefaultPlaceId)
            .Select(s => _locationDtoViewModelMapper.MapToMPoint(s)).ToArray();
        LocationManagementViewModel.Map?.Navigator.SetZoom(points);
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

    private void UpdateFiltersFromHistoryPlace(PlaceViewModel? placeViewModel)
    {
        if (placeViewModel is null)
        {
            SelectedCountry = null;
            SelectedCity = null;
            HistoryViewModel.PlaceViewModel = null;

            InitializeZoom();
            return;
        }

        SelectedCountry = EmptyStringTreeViewConverter.ToUnknown(placeViewModel.Country);
        SelectedCity = EmptyStringTreeViewConverter.ToUnknown(placeViewModel.City);
        HistoryViewModel.PlaceViewModel = placeViewModel;

        LocationManagementViewModel.LoadPlaceViewModel(placeViewModel, false);
        LocationManagementViewModel.ZoomToPointsCommand.Execute(null);
    }

    private void InitializeZoom()
    {
        var points = Places.Where(s => s.Id != PlaceDomain.DefaultPlaceId)
            .Select(s => _locationDtoViewModelMapper.MapToMPoint(s)).ToArray();

        LocationManagementViewModel.PlaceLayer.Clear();
        Task.Run(async () =>
        {
            while (LocationManagementViewModel.Map is null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            LocationManagementViewModel.Map?.Navigator.SetZoom(points);
        });
    }
}