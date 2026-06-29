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
using MyExpenses.Presentation.ViewModels.Systems;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Utils.Converters;

namespace MyExpenses.Presentation.ViewModel;

public partial class RecurringExpenseManagementViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial RecursiveExpenseViewModel RecursiveExpenseViewModel { get; set; } = new();

    public ObservableCollection<RecursiveExpenseViewModel> RecursiveExpenseViewModels { get; } = [];

    public LocationManagementViewModel LocationManagementViewModel { get; }

    public ObservableCollection<AccountViewModel> Accounts { get; } = [];

    public ObservableCollection<CategoryTypeViewModel> CategoryTypes { get; } = [];

    public ObservableCollection<ModePaymentViewModel> ModePaymentViewModels { get; } = [];

    public ObservableCollection<RecursiveFrequencyViewModel> RecursiveFrequencyViewModel { get; } = [];

    private ObservableCollection<PlaceViewModel> Places { get; } = [];

    public ObservableCollection<string> AvailableCountries { get; } = [];

    public ObservableCollection<string> AvailableCities { get; } = [];

    public ObservableCollection<PlaceViewModel> FilteredPlaces { get; } = [];

    [ObservableProperty]
    public partial string? SelectedCountry { get; set; }

    [ObservableProperty]
    public partial string? SelectedCity { get; set; }

    [ObservableProperty]
    public partial bool IsEditRecurringExpense { get; set; }

    public static string TextSearchLocationName { get; } = nameof(PlaceViewModel.Name);

    private bool _isLoaded;

    private readonly IAccountPresentationService _accountPresentationService;

    private readonly IExpensePresentationService _expensePresentationService;

    private readonly ILocationPresentationService _locationPresentationService;

    private readonly IExpenseActionService _expenseActionService;

    private readonly INavigationWindowService _navigationWindowService;

    private readonly INavigationService _navigationService;

    private readonly IDialogService _dialogService;

    private readonly IAccountDtoViewModelMapper _accountDtoViewModelMapper;

    private readonly IExpenseDtoViewModelMapper _expenseDtoViewModelMapper;
    private readonly ISystemPresentationService _systemPresentationService;

    private readonly ILocationDtoViewModelMapper _locationDtoViewModelMapper;

    private readonly ILogger<RecurringExpenseManagementViewModel> _logger;

    public RecurringExpenseManagementViewModel(LocationManagementViewModel locationManagementViewModel,
        IAccountPresentationService accountPresentationService, IExpensePresentationService expensePresentationService,
        ILocationPresentationService locationPresentationService,
        ISystemPresentationService systemPresentationService,
        IExpenseActionService expenseActionService,
        INavigationWindowService navigationWindowService,
        INavigationService navigationService,
        IDialogService dialogService,
        IAccountDtoViewModelMapper accountDtoViewModelMapper,
        IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
        ILocationDtoViewModelMapper locationDtoViewModelMapper,
        ILogger<RecurringExpenseManagementViewModel> logger)
    {
        LocationManagementViewModel = locationManagementViewModel;
        _accountPresentationService = accountPresentationService;
        _expensePresentationService = expensePresentationService;
        _locationPresentationService = locationPresentationService;
        _systemPresentationService = systemPresentationService;
        _expenseActionService = expenseActionService;
        _navigationWindowService = navigationWindowService;
        _navigationService = navigationService;
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

        RecursiveExpenseViewModel?.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(RecursiveExpenseViewModel.PlaceViewModel))
            {
                UpdateFiltersFromRecurringExpensePlace(RecursiveExpenseViewModel.PlaceViewModel);
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
                update: vm => RecursiveExpenseViewModel.AccountViewModel = vm));

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<CategoryTypeViewModel>>(this, (_, m) =>
            OnEntityChanged(m, CategoryTypes, DependencyType.CategoryType,
                getId: vm => vm.Id,
                getName: vm => vm.Name,
                merge: (src, target) => _expenseDtoViewModelMapper.Merge(src, target),
                update: vm => RecursiveExpenseViewModel.CategoryTypeViewModel = vm));

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<ModePaymentViewModel>>(this, (_, m) =>
            OnEntityChanged(m, ModePaymentViewModels, DependencyType.ModePayment,
                getId: vm => vm.Id,
                getName: vm => vm.Name,
                merge: (src, target) => _expenseDtoViewModelMapper.Merge(src, target),
                update: vm => RecursiveExpenseViewModel.ModePaymentViewModel = vm));

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<PlaceViewModel>>(this, (_, m) =>
            OnEntityChanged(m, Places, DependencyType.Place,
                getId: vm => vm.Id,
                getName: vm => vm.Name,
                merge: (src, target) => _locationDtoViewModelMapper.Merge(src, target),
                update: vm => RecursiveExpenseViewModel.PlaceViewModel = vm));

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, (_, m) =>
        {
            OnItemDeleted(m, Accounts, DependencyType.Account, x => x.Id);
            OnItemDeleted(m, CategoryTypes, DependencyType.CategoryType, x => x.Id);
            OnItemDeleted(m, ModePaymentViewModels, DependencyType.ModePayment, x => x.Id);
            OnItemDeleted(m, Places, DependencyType.Place, x => x.Id);
        });
    }
    //
    // [RelayCommand]
    // private async Task OnDelete(CancellationToken cancellationToken = default)
    // {
    //     var result = await _expenseActionService.DeleteHistory(RecursiveExpenseViewModel, cancellationToken);
    //     if (result) _navigationService.GoBack();
    // }

    // [RelayCommand]
    // private async Task OnValid(CancellationToken cancellationToken = default)
    // {
    //     var result = IsEditRecurringExpense
    //         ? await _expenseActionService.UpdateExpense(RecursiveExpenseViewModel, cancellationToken)
    //         : await _expenseActionService.CreateExpense(RecursiveExpenseViewModel, cancellationToken);
    //
    //     if (!result) return;
    //     if (IsEditRecurringExpense)
    //     {
    //         _navigationService.GoBack();
    //         return;
    //     }
    //
    //     var response = _dialogService.ShowMessageBox(ExpenseResources.MessageBoxQuestionNewExpenseCaption,
    //         ExpenseResources.MessageBoxQuestionNewExpenseContent,
    //         MessageBoxButton.YesNo, MsgBoxImage.Question);
    //
    //     if (response is not MessageBoxResult.Yes) _navigationService.GoBack();
    //     RecursiveExpenseViewModel.Reset();
    // }

    // [RelayCommand]
    // private void OnCancel()
    //     => _navigationService.GoBack();

    [RelayCommand]
    private void OnManageAccount()
        => _navigationWindowService.ShowManageAccount(RecursiveExpenseViewModel.AccountViewModel);

    [RelayCommand]
    private void OnManageCategoryType()
        => _navigationWindowService.ShowManageCategoryType(RecursiveExpenseViewModel.CategoryTypeViewModel);

    [RelayCommand]
    private async Task OnManageModePayment(CancellationToken cancellationToken = default)
    {
        if (RecursiveExpenseViewModel.ModePaymentViewModel is null || RecursiveExpenseViewModel.ModePaymentViewModel.CanBeDeleted)
        {
            await _expenseActionService.ManageModePaymentAction(RecursiveExpenseViewModel.ModePaymentViewModel, cancellationToken);
        }
        else
        {
            _dialogService.ShowMessageBox(ExpenseResources.MessageBoxErrorEditDefaultPaymentMethodCaption,
                ExpenseResources.MessageBoxErrorEditDefaultPaymentMethodContent,
                MessageBoxButton.Ok,
                MsgBoxImage.Warning);
        }
    }

    // [RelayCommand]
    // private void OnDateNow()
    //     => HistoryViewModel.Date = DateTime.Now;

    // [RelayCommand]
    // private void OnManagePlace()
    //     => _navigationWindowService.ShowLocationManagementWindow(HistoryViewModel.PlaceViewModel, false);

    [RelayCommand]
    private async Task OnLoad(CancellationToken cancellationToken = default)
    {
        LocationManagementViewModel.LoadCommand.Execute(null);

        var accountsTask = _accountPresentationService.GetAllAccountViewModelAsync(cancellationToken);
        var categoryTypesTask = _expensePresentationService.GetAllCategoryTypeViewModelAsync(cancellationToken);
        var modePaymentsTask = _expensePresentationService.GetAllModePaymentViewModelAsync(cancellationToken);
        var frequencyTask = _systemPresentationService.GetAllFrequencyViewModelAsync(cancellationToken);
        var locationsTask = _locationPresentationService.GetAllPlaces(cancellationToken);

        await Task.WhenAll(accountsTask, categoryTypesTask, modePaymentsTask, frequencyTask, locationsTask);

        Accounts.AddRangeAndSort(accountsTask, x => x.Name!, logger: _logger);
        CategoryTypes.AddRangeAndSort(categoryTypesTask, x => x.Name!, logger: _logger);
        ModePaymentViewModels.AddRangeAndSort(modePaymentsTask, x => x.Name!, logger: _logger);
        RecursiveFrequencyViewModel.AddRangeAndSort(frequencyTask, x => x.Frequency, logger: _logger);
        Places.AddRangeAndSort(locationsTask, x => x.Name!, logger: _logger);

        UpdateAvailableCountries();
        InitializeZoom();

        _isLoaded = true;
    }

    /// <summary>
    /// Updates the AvailableCountries collection based on the data in Places.
    /// </summary>
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

    /// <summary>
    /// Updates the AvailableCities collection based on the selected country.
    /// </summary>
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

    /// <summary>
    /// Updates the FilteredPlaces collection based on the selected country and city.
    /// </summary>
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

    /// <summary>
    /// Handles changes to the selected country and updates the available cities accordingly.
    /// </summary>
    /// <param name="oldValue">The previous value of the selected country.</param>
    /// <param name="newValue">The new value of the selected country.</param>
    partial void OnSelectedCountryChanged(string? oldValue, string? newValue)
    {
        UpdateAvailableCities();
        UpdateFilteredPlaces();
    }

    /// <summary>
    /// Handles changes to the selected city and updates the filtered places accordingly.
    /// </summary>
    /// <param name="oldValue">The previous value of the selected city.</param>
    /// <param name="newValue">The new value of the selected city.</param>
    partial void OnSelectedCityChanged(string? oldValue, string? newValue)
        => UpdateFilteredPlaces();

    /// <summary>
    /// Updates filters based on the history place.
    /// </summary>
    /// <param name="placeViewModel">The place view model to update filters from.</param>
    private void UpdateFiltersFromRecurringExpensePlace(PlaceViewModel? placeViewModel)
    {
        if (placeViewModel is null)
        {
            SelectedCountry = null;
            SelectedCity = null;
            RecursiveExpenseViewModel.PlaceViewModel = null;

            InitializeZoom();
            return;
        }

        SelectedCountry = EmptyStringTreeViewConverter.ToUnknown(placeViewModel.Country);
        SelectedCity = EmptyStringTreeViewConverter.ToUnknown(placeViewModel.City);
        RecursiveExpenseViewModel.PlaceViewModel = placeViewModel;

        LocationManagementViewModel.LoadPlaceViewModel(placeViewModel, false);
        LocationManagementViewModel.ZoomToPointsCommand.Execute(null);
    }

    /// <summary>
    /// Initializes the zoom level of the map based on the locations available.
    /// </summary>
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

    // public void Load(RecursiveExpenseViewModel recursiveExpenseViewModel)
    // {
    //     _expenseDtoViewModelMapper.Merge(historyViewModel, HistoryViewModel);
    //
    //     var accountViewModel = Accounts.FirstOrDefault(c => c.Id == historyViewModel.AccountViewModel?.Id);
    //     if (accountViewModel is not null) HistoryViewModel.AccountViewModel = accountViewModel;
    //
    //     var categoryTypeViewModel = CategoryTypes.FirstOrDefault(c => c.Id == historyViewModel.CategoryTypeViewModel?.Id);
    //     if (categoryTypeViewModel is not null) HistoryViewModel.CategoryTypeViewModel = categoryTypeViewModel;
    //
    //     var modePaymentViewModel = ModePaymentViewModels.FirstOrDefault(m => m.Id == historyViewModel.ModePaymentViewModel?.Id);
    //     if (modePaymentViewModel is not null) HistoryViewModel.ModePaymentViewModel = modePaymentViewModel;
    //
    //     var placeViewModel = Places.FirstOrDefault(p => p.Id == historyViewModel.PlaceViewModel?.Id);
    //     if (placeViewModel is not null) HistoryViewModel.PlaceViewModel = placeViewModel;
    //
    //     RecursiveExpenseViewModel.AcceptChanges();
    //     IsEditRecurringExpense = true;
    // }
}