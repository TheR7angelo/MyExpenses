using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models;
using Domain.Models.Dependencies;
using LiveChartsCore.Kernel.Sketches;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.DashBoardResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Analysis;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Utils.Strings;

namespace MyExpenses.Presentation.ViewModel;

/// <summary>
/// Represents the ViewModel for the dashboard in the MyExpenses application.
/// Provides properties, collections, and utility methods for managing and displaying
/// account and expense data, as well as user-selected filters such as month and year.
/// </summary>
public partial class DashBoardViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the collection of history view models representing individual expense records.
    /// This collection is used to display and manage expense history data in the dashboard.
    /// </summary>
    public ObservableCollection<HistoryViewModel> HistoryViewModels { get; } = [];

    /// <summary>
    /// Gets the collection of total-by-account view models representing summarized data
    /// for individual accounts. This collection is used to display aggregated account-specific
    /// totals and related information on the dashboard.
    /// </summary>
    public ObservableCollection<TotalByAccountViewModel> TotalByAccountViewModels { get; } = [];

    /// <summary>
    /// Gets or sets the selected total-by-account view model, representing the currently active account's summary data.
    /// This property is used for binding and interaction within the dashboard to display or manage account-specific totals.
    /// </summary>
    [ObservableProperty]
    public partial TotalByAccountViewModel? SelectedTotalByAccountViewModel { get; private set; }

    /// <summary>
    /// Gets the collection of category total view models representing the aggregated expense data
    /// for each category. This collection is used to display category-level summaries, such as
    /// percentages, values, and associated colors, in various parts of the dashboard, including
    /// charts or itemized summaries.
    /// </summary>
    public ObservableCollection<CategoryTotalViewModel> CategoryTotalViewModels { get; } = [];

    /// <summary>
    /// Gets or sets the index determining the selected visualization type
    /// for the positive and negative chart values on the dashboard.
    /// This index is used to toggle between different chart modes, such as
    /// monthly positive budget distribution or other comparative views.
    /// </summary>
    [ObservableProperty]
    public partial int IndexOfPositiveNegativeChartValues { get; set; } = 1;

    /// <summary>
    /// Gets the collection of years available for filtering expense data.
    /// This collection is typically populated based on the range of years
    /// present in the user's expense history and is used to allow the user
    /// to select a specific year for viewing or analysis purposes.
    /// </summary>
    public ObservableCollection<string> Years { get; } = [];

    /// <summary>
    /// Gets or sets the selected year from the available list of years.
    /// Used to filter and display data specific to the selected year within the dashboard.
    /// </summary>
    [ObservableProperty]
    public partial string? SelectedYear { get; set; }

    /// <summary>
    /// Gets the collection of month names used to represent the months in a year.
    /// This collection is intended for use in user interface components, such as dropdowns,
    /// to allow users to select or view months in a human-readable format.
    /// The list is updated dynamically based on the current culture's date and time format.
    /// </summary>
    public ObservableCollection<string> Months { get; } = [];

    /// <summary>
    /// Gets or sets the user-selected month from the collection of available months.
    /// This property is used to filter and display data specific to the selected month
    /// in the dashboard interface.
    /// </summary>
    [ObservableProperty]
    public partial string? SelectedMonth { get; set; }

    /// <summary>
    /// Gets an array of tuples representing the sign configurations for chart data filtering.
    /// Each tuple consists of two boolean values:
    /// the first indicates whether positive values should be included, and
    /// the second indicates whether negative values should be included.
    /// This property is used to determine the filtering logic for pie chart data displayed on the dashboard.
    /// </summary>
    private static (bool Positive, bool Negative)[] PositiveNegativeChartValues =>
    [
        (false, true),
        (true, true),
        (true, false)
    ];

    /// <summary>
    /// Manages the configuration and updates of a pie chart visualization.
    /// Responsible for handling the data bindings and interactions between
    /// category totals and the UI representation of the pie chart within the dashboard.
    /// </summary>
    private PieChartManager? PieChartManager { get; set; }

    /// <summary>
    /// Provides access to account-related presentation functionality required by the
    /// dashboard view model. This service is used to retrieve, display, and manage
    /// account-specific data in the application.
    /// </summary>
    private readonly IAccountPresentationService _accountPresentationService;

    /// <summary>
    /// Provides access to expense-related data and operations needed for presentation purposes
    /// within the dashboard. This service is responsible for retrieving expense details,
    /// summary data, and other relevant information to populate and manage the UI elements
    /// of expense-related views.
    /// </summary>
    private readonly IExpensePresentationService _expensePresentationService;

    /// <summary>
    /// Provides access to expense-related actions such as creating, updating,
    /// and deleting expense records. This service is used to perform
    /// business operations on expense data within the dashboard context.
    /// </summary>
    private readonly IExpenseActionService _expenseActionService;

    /// <summary>
    /// Provides functionality to map between DTOs and ViewModels related to account data.
    /// This instance is used within the dashboard to manage data transformations and updates
    /// for account-related ViewModels, ensuring consistency between DTOs and UI-bound objects.
    /// </summary>
    private readonly IAccountDtoViewModelMapper _accountDtoViewModelMapper;

    /// <summary>
    /// Provides functionality to manage navigation and display of various application windows,
    /// including those for managing expenses, accounts, and recurring transactions.
    /// This service is used to abstract and encapsulate logic related to window operations
    /// for improved modularity and testability within the dashboard ViewModel.
    /// </summary>
    private readonly INavigationWindowService _navigationWindowService;

    /// <summary>
    /// Provides navigation functionality within the application, allowing for page transitions,
    /// backward and forward navigation, and route-based navigation with optional parameters.
    /// </summary>
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Provides access to dialog services for displaying message boxes and user prompts
    /// within the dashboard's ViewModel. This service is utilized to interact with the user
    /// through modal dialogs for confirmation, warnings, errors, and other messages.
    /// </summary>
    private readonly IDialogService _dialogService;

    /// <summary>
    /// Provides the core functionality for the dashboard view model.
    /// This class serves as a central point for managing dashboard-related logic,
    /// including account and expense handling, navigation, dialog management,
    /// and message subscriptions.
    /// </summary>
    /// <param name="accountPresentationService">
    /// A service used to interact with account-related presentation logic.
    /// </param>
    /// <param name="expensePresentationService">
    /// A service used to handle expense-related presentation logic.
    /// </param>
    /// <param name="expenseActionService">
    /// A service responsible for performing actions on expenses.
    /// </param>
    /// <param name="accountDtoViewModelMapper">
    /// A mapper that facilitates transformations between account DTOs and view models.
    /// </param>
    /// <param name="navigationWindowService">
    /// A service to manage navigation within the application's window structure.
    /// </param>
    /// <param name="navigationService">
    /// A service to handle in-application navigation tasks.
    /// </param>
    /// <param name="dialogService">
    /// A service for managing dialogs within the application.
    /// </param>
    public DashBoardViewModel(IAccountPresentationService accountPresentationService,
        IExpensePresentationService expensePresentationService,
        IExpenseActionService expenseActionService,
        IAccountDtoViewModelMapper accountDtoViewModelMapper,
        INavigationWindowService navigationWindowService,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _accountPresentationService = accountPresentationService;
        _expensePresentationService = expensePresentationService;
        _expenseActionService = expenseActionService;
        _accountDtoViewModelMapper = accountDtoViewModelMapper;
        _navigationWindowService = navigationWindowService;
        _navigationService = navigationService;
        _dialogService = dialogService;

        RegisterMessage();
    }

    /// <summary>
    /// Registers handlers for various messages used within the application.
    /// Specifically, it subscribes to the following messages:
    /// - <see cref="LanguageChangedMessage"/>, to handle updates related to language changes.
    /// - <see cref="EntityChangedMessage{T}"/> for <see cref="AccountViewModel"/>, to respond to account updates.
    /// - <see cref="EntityChangedMessage{T}"/> for arrays of integers, to handle entity deletions for specific dependencies such as expenses.
    /// </summary>
    private void RegisterMessage()
    {
        WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this, OnLanguageChange);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountViewModel>>(this, OnAccountChange);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, (_, m) =>
        {
            OnItemDeleted(m, HistoryViewModels, DependencyType.Expense, x => x.Id);
            OnAccountDelete(m);
        });
    }

    /// <summary>
    /// Handles the deletion of account entities by processing the provided message.
    /// Removes the corresponding account view models from the collection and updates
    /// the selected account and checked states to reflect the changes.
    /// </summary>
    /// <param name="message">The message containing details about the deleted account entities,
    /// including the entity type, action type, and the IDs of the accounts to be removed.</param>
    private void OnAccountDelete(EntityChangedMessage<int[]> message)
    {
        if (message.Value.EntityType is not DependencyType.Account || message.Value.DataAction is not DataAction.Delete) return;

        var ids = message.Value.Content;
        if (!TotalByAccountViewModels.Any(s => ids.Contains(s.Id))) return;

        foreach (var id in ids)
        {
            var toRemove = TotalByAccountViewModels.First(s => s.Id.Equals(id));
            TotalByAccountViewModels.Remove(toRemove);
            if (SelectedTotalByAccountViewModel == toRemove) SelectedTotalByAccountViewModel = null;
        }

        if (TotalByAccountViewModels.FirstOrDefault(s => s.IsChecked) is not null || !TotalByAccountViewModels.Any()) return;

        var first = TotalByAccountViewModels.First();
        first.IsChecked = true;
        SelectedTotalByAccountViewModel = first;
    }

    /// <summary>
    /// Handles changes to the account data when an account entity is added or updated.
    /// Updates the corresponding view model collection to reflect the changes and ensures
    /// data consistency in the dashboard's display.
    /// </summary>
    /// <param name="recipient">The object that receives the message. Typically, it is the ViewModel instance.</param>
    /// <param name="message">The message containing details about the account change, including the data action and the updated or added account information.</param>
    private async void OnAccountChange(object recipient, EntityChangedMessage<AccountViewModel> message)
    {
        if (message.Value is not {EntityType: DependencyType.Account, Content: var accountViewModel }) return;

        switch (message.Value.DataAction)
        {
            case DataAction.Update:
            {
                var item = TotalByAccountViewModels.FirstOrDefault(s => s.Id == accountViewModel.Id);
                if (item is null) return;
                _accountDtoViewModelMapper.Merge(accountViewModel, item);
                break;
            }
            case DataAction.Add:
            {
                var result = await _accountPresentationService.GetTotalByAccountViewModelAsync(accountViewModel);
                if (!result.IsSuccess) return;
                TotalByAccountViewModels!.AddAndSort(result.Value, s => s!.Name);
                break;
            }
        }
    }

    /// <summary>
    /// Handles the changes required when the application's language is changed.
    /// Updates the list of month names to reflect the new language and ensures the
    /// currently selected month is maintained in the updated list.
    /// </summary>
    /// <param name="recipient">The object that receives the message. Typically, it is the ViewModel instance.</param>
    /// <param name="languageChangedMessage">The message containing information about the language change, including the new language.</param>
    private async void OnLanguageChange(object recipient, LanguageChangedMessage languageChangedMessage)
    {
        if (SelectedMonth is null) await LoadAllMonthName();
        var index = Months.IndexOf(SelectedMonth!);
        await LoadAllMonthName(index + 1);
    }

    /// <summary>
    /// Navigates to the page corresponding to the specified type using the navigation service.
    /// If the provided type is null, the method exits without performing any action.
    /// </summary>
    /// <param name="type">The type representing the target page to navigate to. This can be null.</param>
    [RelayCommand]
    private void OnMovePage(Type? type)
    {
        if (type is null) return;

        _navigationService.Navigate(type.Name);
    }

    /// <summary>
    /// Deletes the specified expense from the expense history using the expense action service.
    /// If the provided item is null, the method exits without performing any action.
    /// </summary>
    /// <param name="item">The history view model representing the expense to be deleted. This can be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the delete operation.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    [RelayCommand]
    private async Task OnDeleteExpense(HistoryViewModel? item, CancellationToken cancellationToken = default)
    {
        if (item is null) return;

        await _expenseActionService.DeleteHistory(item, cancellationToken);
    }

    /// <summary>
    /// Opens the expense management view for the specified expense in the history view model.
    /// If the expense is linked to a bank transfer, the user is prompted for confirmation before proceeding.
    /// </summary>
    /// <param name="item">The history view model representing the expense to be managed. This can be null.</param>
    [RelayCommand]
    private void OnEditExpense(HistoryViewModel? item)
    {
        if (item is null) return;

        if (item.BankTransferViewModel is not null)
        {
            var response = _dialogService.ShowMessageBox(DashBoardResources.MessageBoxUpdateExpenseLindedBankTranferCaption,
                DashBoardResources.MessageBoxUpdateExpenseLindedBankTranferContent,
                MessageBoxButton.YesNoCancel, MsgBoxImage.Question);
            if (response is not MessageBoxResult.Yes) return;
        }

        _navigationWindowService.ManageExpense(item);
    }

    /// <summary>
    /// Toggles the 'IsPointed' status of the specified expense in the history view model and updates the expense status.
    /// If the update operation fails, the 'IsPointed' status is reverted to its original state.
    /// </summary>
    /// <param name="item">The history view model representing the expense to be updated. This can be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the update operation.</param>
    /// <returns>An asynchronous task representing the operation of updating the expense status.</returns>
    [RelayCommand]
    private async Task OnPointedExpense(HistoryViewModel? item, CancellationToken cancellationToken = default)
    {
        if (item is null) return;
        item.AcceptChanges();

        item.IsPointed = !item.IsPointed;
        var result = await _expenseActionService.UpdateExpense(item, cancellationToken);
        if (!result) item.IsPointed = !item.IsPointed;
    }

    /// <summary>
    /// Initializes and loads data into the pie chart component based on the current year and month.
    /// This method sets up the pie chart manager and retrieves any necessary records for chart visualization.
    /// </summary>
    /// <param name="pieChartView">The interface representing the pie chart to be managed and updated.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the data-loading operation.</param>
    /// <returns>An asynchronous task representing the operation of loading and preparing the pie chart data.</returns>
    [RelayCommand]
    private async Task OnLoadPieChart(IPieChartView pieChartView, CancellationToken cancellationToken = default)
    {
        PieChartManager = new PieChartManager(pieChartView, CategoryTotalViewModels);

        var (currentYear, currentMonth, _) = DateTime.Now;
        await LoadPieChartRecords(currentYear, currentMonth, cancellationToken);
    }

    /// <summary>
    /// Loads and synchronizes the data for the chart and grid records on the dashboard.
    /// Executes multiple asynchronous tasks in parallel to fetch and update the expense records
    /// and pie chart data based on the selected filters, such as year and month.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the tasks to complete.</param>
    /// <returns>An asynchronous task representing the data loading operation.</returns>
    [RelayCommand]
    private async Task OnManageMoveChartAndGridRecords(CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>
        {
            LoadExpenseRecord(cancellationToken), LoadPieChartRecords(cancellationToken: cancellationToken)
        };
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Adjusts the filter date by moving it forward or backward by the specified number of months.
    /// Extracts the current selected month and year from user inputs, defaults to the current date if
    /// selection is null, and then computes the new date by applying the specified delta in months.
    /// Finally, updates the dashboard filters with the computed date.
    /// </summary>
    /// <param name="deltaMonth">The number of months to move the filter date. A positive value moves forward, and a negative value moves backward.</param>
    [RelayCommand]
    private void OnManageMoveMonth(int deltaMonth)
    {
        var (year, month) = ExtractMonthAndYearFromSelection();
        year ??= DateTime.Now.Year;
        month ??= DateTime.Now.Month;

        var dateOnly = new DateOnly((int)year, (int)month, 1);
        dateOnly = dateOnly.AddMonths(deltaMonth);

        UpdateFilterDate(dateOnly);
    }

    /// <summary>
    /// Updates the filter date to the current month and year by creating a `DateOnly` instance
    /// representing the first day of the current month. Invokes the `UpdateFilterDate` method
    /// with the generated date to adjust the dashboard filters accordingly.
    /// </summary>
    [RelayCommand]
    private void OnManageDateNow()
    {
        var dateOnly = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
        UpdateFilterDate(dateOnly);
    }

    /// <summary>
    /// Updates the filter date for the dashboard view, modifying the selected year and month accordingly.
    /// Ensures the year exists within the permissible data range and displays a warning if the selection exceeds available data.
    /// </summary>
    /// <param name="dateOnly">The new date to set as the filter, represented as a `DateOnly` object.</param>
    private void UpdateFilterDate(DateOnly dateOnly)
    {
        var (year, _) = ExtractMonthAndYearFromSelection();
        year ??= DateTime.Now.Year;

        if (dateOnly.Year != year && !Years.Contains(dateOnly.Year.ToString()))
        {
            _dialogService.ShowMessageBox("Warning", "No more data are available",
                MessageBoxButton.Ok, MsgBoxImage.Warning);
            return;
        }

        SelectedYear = dateOnly.Year.ToString();
        SelectedMonth = Months[dateOnly.Month - 1];
    }

    /// <summary>
    /// Handles the selection of a specific account's total information and triggers the loading of related expense records.
    /// Updates the selected account to provide focused data for the user interface.
    /// </summary>
    /// <param name="totalByAccountViewModel">The account view model containing the total information to process.</param>
    /// <param name="cancellationToken">An optional cancellation token for observing the operation's lifecycle.</param>
    /// <returns>A task representing the asynchronous operation for handling the account selection and loading expenses.</returns>
    [RelayCommand]
    private async Task OnTotalByAccountCheck(TotalByAccountViewModel totalByAccountViewModel,
        CancellationToken cancellationToken = default)
    {
        SelectedTotalByAccountViewModel = totalByAccountViewModel;
        await LoadExpenseRecord(cancellationToken);
    }

    /// <summary>
    /// Handles the loading of dashboard data, including recurring expenses, yearly and monthly expenses,
    /// total expenses by account, and pie chart records. Executes these operations concurrently for efficiency
    /// and updates the dashboard state upon completion.
    /// </summary>
    /// <param name="cancellationToken">An optional cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation of loading dashboard data.</returns>
    [RelayCommand]
    private async Task OnLoad(CancellationToken cancellationToken = default)
    {
        var (currentYear, currentMonth, _) = DateTime.Now;

        var tasks = new List<Task>
        {
            LoadRecurringExpense(currentYear, currentMonth, cancellationToken),
            LoadAllExpenseYear(currentYear, cancellationToken),
            LoadAllMonthName(currentMonth), LoadTotalByAccount(cancellationToken)
        };
        await Task.WhenAll(tasks);

        await LoadExpenseRecord(cancellationToken);
        await LoadPieChartRecords(currentYear, currentMonth, cancellationToken);
    }

    /// <summary>
    /// Loads pie chart data for the currently selected account and updates the pie chart visualization. Filters the data
    /// by the specified year and month if provided, or defaults to the current selections derived from the account.
    /// Displays an error message if the data retrieval fails.
    /// </summary>
    /// <param name="year">The year to filter the pie chart data. Defaults to the selected or current year if not specified.</param>
    /// <param name="month">The month to filter the pie chart data. Defaults to the selected or current month if not specified.</param>
    /// <param name="cancellationToken">An optional cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation of loading pie chart data.</returns>
    private async Task LoadPieChartRecords(int? year = null, int? month = null,
        CancellationToken cancellationToken = default)
    {
        if (SelectedTotalByAccountViewModel is null || PieChartManager is null) return;

        var (currentYear, currentMonth) = ExtractMonthAndYearFromSelection();
        year ??= currentYear;
        month ??= currentMonth;

        var results = await _expensePresentationService.GetAllDetailTotalCategories(SelectedTotalByAccountViewModel.Id, year, month, cancellationToken);
        if (results.IsSuccess)
        {
            var (positive, negative) = PositiveNegativeChartValues[IndexOfPositiveNegativeChartValues];
            var categoriesTotals = results.Value!.AggregateCategoryTotalsBySign(out var grandTotal, positive, negative);

            PieChartManager.UpdateChartUi(categoriesTotals, grandTotal);
        }
        else
        {
            _dialogService.ShowMessageBox(DashBoardResources.MessageBoxLoadingAllDetailTotalCategoriesErrorCaption,
                DashBoardResources.MessageBoxLoadingAllDetailTotalCategoriesErrorContent,
                MessageBoxButton.Ok, MsgBoxImage.Error);
        }
    }

    /// <summary>
    /// Loads expense records associated with the currently selected account. Filters the expenses
    /// by the year and month derived from the selected account, if available. Updates the
    /// history view model with the retrieved records or displays an error message in case of failure.
    /// </summary>
    /// <param name="cancellationToken">An optional cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation of loading expense records.</returns>
    [RelayCommand]
    private async Task LoadExpenseRecord(CancellationToken cancellationToken = default)
    {
        if (SelectedTotalByAccountViewModel is null) return;

        var (year, month) = ExtractMonthAndYearFromSelection();
        var result = await _expensePresentationService.GetAllExpenses(SelectedTotalByAccountViewModel.Id,
            year, month, cancellationToken);

        HistoryViewModels.Clear();

        if (result.IsSuccess)
        {
            var records = result.Value!.OrderByDescending(s => s.Date);
            HistoryViewModels.AddRange(records);
        }
        else
        {
            _dialogService.ShowMessageBox(DashBoardResources.MessageBoxLoadingAllExpenseRecordErrorCaption,
                DashBoardResources.MessageBoxLoadingAllExpenseRecordErrorContent,
                MessageBoxButton.Ok, MsgBoxImage.Error);
        }
    }

    /// <summary>
    /// Extracts the year and month from the selected year and month properties.
    /// If <c>SelectedMonth</c> is non-null, its index within the predefined list of months is
    /// converted to a corresponding numerical representation (1-based). The <c>SelectedYear</c>
    /// value is parsed to an integer if possible. Returns null for any component that cannot be derived.
    /// </summary>
    /// <returns>A tuple containing the extracted year and month. The year and/or month can be null if
    /// the corresponding values are not set or cannot be parsed.</returns>
    private (int? Year, int? Month) ExtractMonthAndYearFromSelection()
    {
        int? monthInt = null;
        if (!string.IsNullOrEmpty(SelectedMonth))
        {
            monthInt = Months.IndexOf(SelectedMonth) + 1;
        }

        _ = int.TryParse(SelectedYear, out var yearInt);

        return (yearInt, monthInt);
    }

    /// <summary>
    /// Loads the total amounts grouped by account into the TotalByAccountViewModels collection.
    /// If the operation is successful, the collection is cleared and populated with the latest data,
    /// then sorted by account name. Sets the SelectedTotalByAccountViewModel to the first account
    /// and marks it as checked. Displays an error message if the operation fails.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation of the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task LoadTotalByAccount(CancellationToken cancellationToken = default)
    {
        var result = await _accountPresentationService.GetAllTotalByAccountViewModelAsync(cancellationToken);
        if (result.IsSuccess)
        {
            TotalByAccountViewModels.Clear();
            TotalByAccountViewModels.AddRangeAndSort(result.Value!, s => s.Name);
            SelectedTotalByAccountViewModel = TotalByAccountViewModels.FirstOrDefault();
            SelectedTotalByAccountViewModel?.IsChecked = true;
        }
        else
        {
            _dialogService.ShowMessageBox(DashBoardResources.MessageBoxLoadingAccountTotalErrorCaption,
                DashBoardResources.MessageBoxLoadingAccountTotalErrorContent,
                MessageBoxButton.Ok, MsgBoxImage.Error);
        }
    }

    /// <summary>
    /// Loads all month names from the current culture into the Months collection. If the collection
    /// is empty, it populates it with the localized month names and optionally selects a specific
    /// month. If the collection is already populated, it updates the month names while retaining
    /// the currently selected month.
    /// </summary>
    /// <param name="currentMonth">The current month (1-based index) to select after loading the month names. If null, no specific month is selected.</param>
    /// <returns>A task that represents the completion of the operation.</returns>
    private Task LoadAllMonthName(int? currentMonth = null)
    {
        var culture = CultureInfo.CurrentCulture;

        var newMonths = culture.DateTimeFormat.MonthNames
            .Where(m => !string.IsNullOrEmpty(m))
            .Select(m => m.ToFirstCharUpper())
            .ToList();

        if (Months.Count is 0)
        {
            Months.AddRange(newMonths);
            if (currentMonth is null) return Task.CompletedTask;
            SelectedMonth = Months[(int)currentMonth - 1];

            return Task.CompletedTask;
        }

        int? selected = SelectedMonth is null
            ? null
            : Months.IndexOf(SelectedMonth);

        for (var i = 0; i < newMonths.Count; i++)
        {
            Months[i] = newMonths[i];
        }

        if (selected is null) return Task.CompletedTask;
        SelectedMonth = Months[selected.Value];

        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads all the expense years available and updates the collection of years
    /// in descending order. If no expense years are found, it adds the current year
    /// to the collection. Updates the SelectedYear property to the current year.
    /// </summary>
    /// <param name="currentYear">The current year to be used as a reference if no expense years are found or additional years need to be added.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task LoadAllExpenseYear(int currentYear, CancellationToken cancellationToken)
    {
        var results = await _expensePresentationService.GetAllExpenseYear(SortOrder.Descending, cancellationToken);
        if (results.IsSuccess)
        {
            var years = results.Value!.ToList();

            if (years.Count is 0) years.Add(currentYear);
            var lastYear = years.Max();

            if (lastYear < currentYear)
            {
                for (var y = lastYear + 1; y <= currentYear; y++)
                {
                    years.Add(y);
                }
            }
            years.Sort((a, b) => b.CompareTo(a));
            Years.AddRange(years.Select(y => y.ToString()));
            SelectedYear = currentYear.ToString();
        }
    }

    /// <summary>
    /// Loads the recurring expenses for a specific year and month and handles the necessary UI updates or error reporting.
    /// Opens the recurring expense window if recurring expenses are found. Logs and displays an error message if the operation fails.
    /// </summary>
    /// <param name="currentYear">The year for which the recurring expenses are to be fetched.</param>
    /// <param name="currentMonth">The month for which the recurring expenses are to be fetched.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task LoadRecurringExpense(int currentYear, int currentMonth,
        CancellationToken cancellationToken = default)
    {
        var results = await _expensePresentationService.GetAllActiveRecurringExpense(currentYear, currentMonth, cancellationToken);
        if (results.IsSuccess)
        {
            if (results.Value!.Any()) _navigationWindowService.ShowRecurringExpenseWindow();
        }
        else
        {
            _dialogService.ShowMessageBox(DashBoardResources.MessageBoxLoadingAllRecurringExpenseErrorCaption,
                DashBoardResources.MessageBoxLoadingAllRecurringExpenseErrorContent,
                MessageBoxButton.Ok, MsgBoxImage.Error);
        }
    }

    /// <summary>
    /// Updates the index of the positive and negative chart values based on the provided value,
    /// and reloads the pie chart records asynchronously.
    /// </summary>
    /// <param name="valueToAdd">The value to add to the current index of positive and negative chart values.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [RelayCommand]
    private async Task OnManagePieChart(int valueToAdd, CancellationToken cancellationToken = default)
    {
        IndexOfPositiveNegativeChartValues =
            (IndexOfPositiveNegativeChartValues + valueToAdd + PositiveNegativeChartValues.Length)
            % PositiveNegativeChartValues.Length;

        await LoadPieChartRecords(cancellationToken: cancellationToken);
    }
}