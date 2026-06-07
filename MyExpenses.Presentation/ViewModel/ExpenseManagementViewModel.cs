using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
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
    public partial PlaceViewModel? SelectedPlace { get; set; }

    [ObservableProperty]
    public partial bool IsHistoryEdit { get; set; }

    public static string TextSearchLocationName { get; } = nameof(PlaceViewModel.Name);

    private readonly IAccountPresentationService _accountPresentationService;
    private readonly IExpensePresentationService _expensePresentationService;
    private readonly ILocationPresentationService _locationPresentationService;
    private readonly IExpenseActionService _expenseActionService;
    private readonly INavigationWindowService _navigationWindowService;
    private readonly IDialogService _dialogService;
    private readonly IAccountDtoViewModelMapper _accountDtoViewModelMapper;
    private readonly IExpenseDtoViewModelMapper _expenseDtoViewModelMapper;

    private readonly ILogger<ExpenseManagementViewModel> _logger;

    public ExpenseManagementViewModel(LocationManagementViewModel locationManagementViewModel,
        IAccountPresentationService accountPresentationService, IExpensePresentationService expensePresentationService,
        ILocationPresentationService locationPresentationService,
        IExpenseActionService expenseActionService,
        INavigationWindowService navigationWindowService,
        IDialogService dialogService,
        IAccountDtoViewModelMapper accountDtoViewModelMapper,
        IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
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
                updateHistory: vm => HistoryViewModel.AccountViewModel = vm));

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<CategoryTypeViewModel>>(this, (r, m) =>
            OnEntityChanged(m, CategoryTypes, DependencyType.CategoryType,
                getId: vm => vm.Id,
                getName: vm => vm.Name,
                merge: (src, target) => _expenseDtoViewModelMapper.Merge(src, target),
                updateHistory: vm => HistoryViewModel.CategoryTypeViewModel = vm));

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<ModePaymentViewModel>>(this, (r, m) =>
            OnEntityChanged(m, ModePaymentViewModels, DependencyType.ModePayment,
                getId: vm => vm.Id,
                getName: vm => vm.Name,
                merge: (src, target) => _expenseDtoViewModelMapper.Merge(src, target),
                updateHistory: vm => HistoryViewModel.ModePaymentViewModel = vm));

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, OnItemDeleted);
    }

    private void OnEntityChanged<T>(
        EntityChangedMessage<T> m,
        ObservableCollection<T> collection,
        DependencyType expectedType,
        Func<T, int> getId,
        Func<T, string?> getName,
        Action<T, T> merge,
        Action<T> updateHistory) where T : class
    {
        if (m.Value.EntityType != expectedType) return;

        var content = m.Value.Content;

        switch (m.Value.DataAction)
        {
            case DataAction.Update:
                var item = collection.FirstOrDefault(s => getId(s) == getId(content));
                if (item is not null)
                    merge(content, item);
                break;

            case DataAction.Add:
                collection.AddAndSort(content, s => getName(s)!);
                updateHistory(content);
                break;
        }
    }

    private void OnItemDeleted(object recipient, EntityChangedMessage<int[]> m)
    {
        if (m.Value.DataAction is not DataAction.Delete) return;

        switch (m.Value.EntityType)
        {
            case DependencyType.Account:
                ApplyDelete(m.Value.Content, Accounts, x => x.Id);
                break;
            case DependencyType.CategoryType:
                ApplyDelete(m.Value.Content, CategoryTypes, x => x.Id);
                break;
            case DependencyType.ModePayment:
                ApplyDelete(m.Value.Content, ModePaymentViewModels, x => x.Id);
                break;
        }
    }

    private void ApplyDelete<T>(int[] ids, ICollection<T> collection, Func<T, int> getId)
    {
        var toDeletes = collection.Where(s => ids.Contains(getId(s))).ToList();
        collection.RemoveRange(toDeletes);
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