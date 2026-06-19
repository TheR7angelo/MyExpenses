using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models;
using Domain.Models.Dependencies;
using LiveChartsCore.Kernel.Sketches;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Analysis;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Utils.Strings;

namespace MyExpenses.Presentation.ViewModel;

public partial class DashBoardViewModel : ViewModelBase
{
    public ObservableCollection<HistoryViewModel> HistoryViewModels { get; } = [];

    public ObservableCollection<TotalByAccountViewModel> TotalByAccountViewModels { get; } = [];

    [ObservableProperty]
    public partial TotalByAccountViewModel? SelectedTotalByAccountViewModel { get; set; }

    public ObservableCollection<CategoryTotalViewModel> CategoryTotalViewModels { get; } = [];

    [ObservableProperty]
    public partial int IndexOfPositiveNegativeChartValues { get; set; } = 1;

    public ObservableCollection<string> Years { get; } = [];

    [ObservableProperty]
    public partial string? SelectedYear { get; set; }
    public ObservableCollection<string> Months { get; } = [];

    [ObservableProperty]
    public partial string? SelectedMonth { get; set; }

    private static (bool Positive, bool Negative)[] PositiveNegativeChartValues =>
    [
        (false, true),
        (true, true),
        (true, false)
    ];

    private PieChartManager? PieChartManager { get; set; }

    private readonly IAccountPresentationService _accountPresentationService;
    private readonly IExpensePresentationService _expensePresentationService;
    private readonly IAccountDtoViewModelMapper _accountDtoViewModelMapper;
    private readonly INavigationWindowService _navigationWindowService;
    private readonly IDialogService _dialogService;
    private readonly ILogger<DashBoardViewModel> _logger;

    public DashBoardViewModel(IAccountPresentationService accountPresentationService,
        IExpensePresentationService expensePresentationService,
        IAccountDtoViewModelMapper accountDtoViewModelMapper,
        INavigationWindowService navigationWindowService,
        IDialogService dialogService, ILogger<DashBoardViewModel> logger)
    {
        _accountPresentationService = accountPresentationService;
        _expensePresentationService = expensePresentationService;
        _accountDtoViewModelMapper = accountDtoViewModelMapper;
        _navigationWindowService = navigationWindowService;
        _dialogService = dialogService;
        _logger = logger;

        RegisterMessage();
    }

    private void RegisterMessage()
    {
        WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this, OnLanguageChange);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountViewModel>>(this, OnAccountChange);
    }

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

    private async void OnLanguageChange(object recipient, LanguageChangedMessage languageChangedMessage)
    {
        if (SelectedMonth is null) await LoadAllMonthName();
        var index = Months.IndexOf(SelectedMonth!);
        await LoadAllMonthName(index + 1);
    }

    [RelayCommand]
    private async Task OnLoadPieChart(IPieChartView pieChartView, CancellationToken cancellationToken = default)
    {
        PieChartManager = new PieChartManager(pieChartView, CategoryTotalViewModels);

        var (currentYear, currentMonth, _) = DateTime.Now;
        await LoadPieChartRecords(currentYear, currentMonth, cancellationToken);
    }

    [RelayCommand]
    private async Task OnManageMoveChartAndGridRecords(CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>
        {
            LoadExpenseRecord(cancellationToken), LoadPieChartRecords(cancellationToken: cancellationToken)
        };
        await Task.WhenAll(tasks);
    }

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

    [RelayCommand]
    private void OnManageDateNow()
    {
        var dateOnly = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
        UpdateFilterDate(dateOnly);
    }

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

    [RelayCommand]
    private async Task OnTotalByAccountCheck(TotalByAccountViewModel totalByAccountViewModel, CancellationToken cancellationToken = default)
    {
        SelectedTotalByAccountViewModel = totalByAccountViewModel;
        await LoadExpenseRecord(cancellationToken);
    }

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
            // TODO trad
            _dialogService.ShowMessageBox("Error",
                "An error occurred when trying to load the detail total category record. Please try again later.",
                MessageBoxButton.Ok, MsgBoxImage.Error);
        }
    }

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
            HistoryViewModels.AddRange(result.Value!);
        }
        else
        {
            // TODO trad
            _dialogService.ShowMessageBox("Error",
                "An error occurred when trying to load the expenses record. Please try again later.",
                MessageBoxButton.Ok, MsgBoxImage.Error);
        }
    }

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
            // TODO trad
            _dialogService.ShowMessageBox("Error", "Can't load total by account. Try again later", MessageBoxButton.Ok, MsgBoxImage.Error);
        }
    }

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

    private async Task LoadRecurringExpense(int currentYear, int currentMonth, CancellationToken cancellationToken = default)
    {
        var results = await _expensePresentationService.GetAllActiveRecurrences(currentYear, currentMonth, cancellationToken);
        if (results.IsSuccess)
        {
            if (results.Value!.Any()) _navigationWindowService.ShowRecurringExpenseWindow();
        }
        else
        {
            _logger.LogError("Error loading recurring expenses for current month and year");
            _dialogService.ShowMessageBox("Error", "Error loading recurring expenses for current month and year",
                MessageBoxButton.Ok, MsgBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task OnManagePieChart(int valueToAdd, CancellationToken cancellationToken = default)
    {
        IndexOfPositiveNegativeChartValues =
            (IndexOfPositiveNegativeChartValues + valueToAdd + PositiveNegativeChartValues.Length)
            % PositiveNegativeChartValues.Length;

        await LoadPieChartRecords(cancellationToken: cancellationToken);
    }
}