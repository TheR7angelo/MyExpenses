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

/// <summary>
/// ViewModel for managing expenses.
/// Contains properties and methods to handle expense data and interactions.
/// Inherits from ViewModelBase.
/// </summary>
public partial class ExpenseManagementViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial RecursiveExpenseViewModel RecursiveExpenseViewModel { get; set; } = new();

    public ObservableCollection<RecursiveExpenseViewModel> RecursiveExpenseViewModels { get; } = [];

    /// <summary>View model for managing expense history.</summary>
    [ObservableProperty]
    public partial HistoryViewModel HistoryViewModel { get; private set; } = new();

    /// <summary>View model managing locations for expense management.</summary>
    public LocationManagementViewModel LocationManagementViewModel { get; }

    /// <summary>Collection of account view models representing different accounts available for expenses.</summary>
    public ObservableCollection<AccountViewModel> Accounts { get; } = [];

    /// <summary>Collection of category type view models representing different categories available for expenses.</summary>
    public ObservableCollection<CategoryTypeViewModel> CategoryTypes { get; } = [];

    /// <summary>Collection of mode payment view models representing different payment methods available for expenses.</summary>
    public ObservableCollection<ModePaymentViewModel> ModePaymentViewModels { get; } = [];

    /// <summary>Collection of place view models representing locations available for management.</summary>
    private ObservableCollection<PlaceViewModel> Places { get; } = [];

    /// <summary>The collection of countries available for selection in the user interface.</summary>
    public ObservableCollection<string> AvailableCountries { get; } = [];

    /// <summary>The collection of cities available for selection based on the selected country.</summary>
    public ObservableCollection<string> AvailableCities { get; } = [];

    /// <summary>The collection of filtered places based on the selected country and city.</summary>
    public ObservableCollection<PlaceViewModel> FilteredPlaces { get; } = [];

    /// <summary>The country selected in the location management.</summary>
    [ObservableProperty]
    public partial string? SelectedCountry { get; set; }

    /// <summary>The city selected in the location management.</summary>
    [ObservableProperty]
    public partial string? SelectedCity { get; set; }

    /// <summary>Indicates whether the current operation is an edit of a expense record.</summary>
    [ObservableProperty]
    public partial bool IsEditExpense { get; set; }

    /// <summary>Stores the name of the property to be used for text searching in the LocationViewModel.</summary>
    public static string TextSearchLocationName { get; } = nameof(PlaceViewModel.Name);

    /// <summary>Indicates whether the data has been loaded into the ViewModel.</summary>
    private bool _isLoaded;

    /// <summary>
    /// Represents the service responsible for presenting account-related data and functionality.
    /// This includes retrieving, updating, and filtering accounts based on various criteria.
    /// </summary>/// <summary>
    /// Represents the service responsible for presenting account-related data and functionality.
    /// This includes retrieving, updating, and filtering accounts based on various criteria.
    /// </summary>
    private readonly IAccountPresentationService _accountPresentationService;

    /// <summary>
    /// Represents the service responsible for presenting expense-related data and functionality.
    /// This includes retrieving, updating, and filtering expenses based on various criteria.
    /// </summary>
    private readonly IExpensePresentationService _expensePresentationService;

    /// <summary>
    /// Represents the service responsible for presenting location-related data and functionality.
    /// This includes retrieving, updating, and filtering places based on various criteria.
    /// </summary>
    private readonly ILocationPresentationService _locationPresentationService;

    /// <summary>
    /// Represents the service responsible for performing various actions related to expenses.
    /// This includes deleting, updating, and creating expense records.
    /// </summary>
    private readonly IExpenseActionService _expenseActionService;

    /// <summary>
    /// Represents the service responsible for managing navigation within the application.
    /// Used to show and hide different windows or manage the navigation stack.
    /// </summary>
    private readonly INavigationWindowService _navigationWindowService;

    /// <summary>
    /// Represents the service responsible for managing navigation within the application.
    /// Used to navigate between different views and manage the navigation stack.
    /// </summary>
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Represents the service responsible for displaying dialog boxes and message boxes.
    /// Used in various parts of the application to provide user feedback and interact with the user through modal dialogs.
    /// </summary>
    private readonly IDialogService _dialogService;

    /// <summary>
    /// Represents the mapper responsible for converting data transfer objects (DTOs) related to accounts into view models.
    /// Used in the ExpenseManagementViewModel to facilitate data transformation between DTOs and view models, particularly when dealing with account-related operations.
    /// </summary>
    private readonly IAccountDtoViewModelMapper _accountDtoViewModelMapper;

    /// <summary>
    /// Represents the mapper responsible for converting data transfer objects (DTOs) related to expenses into view models.
    /// Used in the ExpenseManagementViewModel to facilitate data transformation between DTOs and view models.
    /// </summary>
    private readonly IExpenseDtoViewModelMapper _expenseDtoViewModelMapper;

    /// <summary>
    /// Represents the mapper responsible for converting data transfer objects (DTOs) related to locations into view models.
    /// Used in the ExpenseManagementViewModel to facilitate data transformation between DTOs and view models.
    /// </summary>
    private readonly ILocationDtoViewModelMapper _locationDtoViewModelMapper;

    /// <summary>
    /// Represents the logging service used for recording information, warnings, and errors in the ExpenseManagementViewModel.
    /// </summary>
    private readonly ILogger<ExpenseManagementViewModel> _logger;

    /// <summary>
    /// ViewModel for managing expenses.
    /// </summary>
    public ExpenseManagementViewModel(LocationManagementViewModel locationManagementViewModel,
        IAccountPresentationService accountPresentationService, IExpensePresentationService expensePresentationService,
        ILocationPresentationService locationPresentationService,
        IExpenseActionService expenseActionService,
        INavigationWindowService navigationWindowService,
        INavigationService navigationService,
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

        HistoryViewModel?.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(HistoryViewModel.PlaceViewModel))
            {
                UpdateFiltersFromHistoryPlace(HistoryViewModel.PlaceViewModel);
            }
        };

        RegisterMessages();
    }

    /// <summary>
    /// Registers message handlers to handle entity changes and item deletions.
    /// </summary>
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

    /// <summary>
    /// Deletes the current expense entry.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    [RelayCommand]
    private async Task OnDelete(CancellationToken cancellationToken = default)
    {
        var result = await _expenseActionService.DeleteHistory(HistoryViewModel, cancellationToken);
        if (result) _navigationService.GoBack();
    }

    /// <summary>
    /// Validates the current expense entry and performs the appropriate action.
    /// If IsHistoryEdit is true, updates the existing expense.
    /// Otherwise, creates a new expense.
    /// After successful validation or update, navigates back to the previous screen if applicable.
    /// Displays a message box to confirm the creation of a new expense.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    [RelayCommand]
    private async Task OnValid(CancellationToken cancellationToken = default)
    {
        var result = IsEditExpense
            ? await _expenseActionService.UpdateExpense(HistoryViewModel, cancellationToken)
            : await _expenseActionService.CreateExpense(HistoryViewModel, cancellationToken);

        if (!result) return;
        if (IsEditExpense)
        {
            _navigationService.GoBack();
            return;
        }

        var response = _dialogService.ShowMessageBox(ExpenseResources.MessageBoxQuestionNewExpenseCaption,
            ExpenseResources.MessageBoxQuestionNewExpenseContent,
            MessageBoxButton.YesNo, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) _navigationService.GoBack();
        HistoryViewModel.Reset();
    }

    /// <summary>
    /// Navigates back to the previous entry in the navigation history, if possible.
    /// </summary>
    [RelayCommand]
    private void OnCancel()
        => _navigationService.GoBack();

    /// <summary>
    /// Navigates to the window for managing accounts.
    /// </summary>
    [RelayCommand]
    private void OnManageAccount()
        => _navigationWindowService.ShowManageAccount(HistoryViewModel.AccountViewModel);

    /// <summary>
    /// Navigates to the window for managing category types.
    /// </summary>
    [RelayCommand]
    private void OnManageCategoryType()
        => _navigationWindowService.ShowManageCategoryType(HistoryViewModel.CategoryTypeViewModel);

    /// <summary>
    /// Manages the payment mode in the history view model.
    /// </summary>
    /// <param name="cancellationToken">A token to allow cancellation of the operation.</param>
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

    /// <summary>
    /// Sets the date in the history view model to the current date and time.
    /// </summary>
    [RelayCommand]
    private void OnDateNow()
        => HistoryViewModel.Date = DateTime.Now;

    /// <summary>
    /// Navigates to the location management window.
    /// </summary>
    [RelayCommand]
    private void OnManagePlace()
        => _navigationWindowService.ShowLocationManagementWindow(HistoryViewModel.PlaceViewModel, false);

    /// <summary>
    /// Loads data asynchronously when the view model is initialized.
    /// </summary>
    /// <param name="cancellationToken">Token for canceling the operation.</param>
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

    /// <summary>
    /// Loads data from the provided history view model into the current instance's history view model.
    /// </summary>
    /// <param name="historyViewModel">The source history view model containing the data to be loaded.</param>
    public void Load(HistoryViewModel historyViewModel)
    {
        _expenseDtoViewModelMapper.Merge(historyViewModel, HistoryViewModel);

        var accountViewModel = Accounts.FirstOrDefault(c => c.Id == historyViewModel.AccountViewModel?.Id);
        if (accountViewModel is not null) HistoryViewModel.AccountViewModel = accountViewModel;

        var categoryTypeViewModel = CategoryTypes.FirstOrDefault(c => c.Id == historyViewModel.CategoryTypeViewModel?.Id);
        if (categoryTypeViewModel is not null) HistoryViewModel.CategoryTypeViewModel = categoryTypeViewModel;

        var modePaymentViewModel = ModePaymentViewModels.FirstOrDefault(m => m.Id == historyViewModel.ModePaymentViewModel?.Id);
        if (modePaymentViewModel is not null) HistoryViewModel.ModePaymentViewModel = modePaymentViewModel;

        var placeViewModel = Places.FirstOrDefault(p => p.Id == historyViewModel.PlaceViewModel?.Id);
        if (placeViewModel is not null) HistoryViewModel.PlaceViewModel = placeViewModel;

        HistoryViewModel.AcceptChanges();
        IsEditExpense = true;
    }
}