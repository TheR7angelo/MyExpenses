using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.Mappings;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Utils.Strings;

namespace MyExpenses.Presentation.ViewModel;

public partial class DashBoardViewModel : ViewModelBase
{
    public ObservableCollection<TotalByAccountViewModel> TotalByAccountViewModels { get; } = [];

    [ObservableProperty]
    public partial TotalByAccountViewModel? SelectedTotalByAccountViewModel { get; set; }

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

    private readonly IAccountPresentationService _accountPresentationService;
    private readonly IExpensePresentationService _expensePresentationService;
    private readonly IExpenseDtoDomainMapper _expenseDtoDomainMapper;
    private readonly IExpenseDtoViewModelMapper _expenseDtoViewModelMapper;
    private readonly IAccountDtoViewModelMapper _accountDtoViewModelMapper;
    private readonly INavigationWindowService _navigationWindowService;
    private readonly IDialogService _dialogService;
    private readonly ILogger<DashBoardViewModel> _logger;

    public DashBoardViewModel(IAccountPresentationService accountPresentationService,
        IExpensePresentationService expensePresentationService,
        IExpenseDtoDomainMapper expenseDtoDomainMapper,
        IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
        IAccountDtoViewModelMapper accountDtoViewModelMapper,
        INavigationWindowService navigationWindowService,
        IDialogService dialogService, ILogger<DashBoardViewModel> logger)
    {
        _accountPresentationService = accountPresentationService;
        _expensePresentationService = expensePresentationService;
        _expenseDtoDomainMapper = expenseDtoDomainMapper;
        _expenseDtoViewModelMapper = expenseDtoViewModelMapper;
        _accountDtoViewModelMapper = accountDtoViewModelMapper;
        _navigationWindowService = navigationWindowService;
        _dialogService = dialogService;
        _logger = logger;

        RegisterMessage();
    }

    private void RegisterMessage()
    {
        WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this, OnLanguageChange);
    }

    private async void OnLanguageChange(object recipient, LanguageChangedMessage languageChangedMessage)
    {
        if (SelectedMonth is null) await LoadAllMonthName();
        var index = Months.IndexOf(SelectedMonth!);
        await LoadAllMonthName(index + 1);
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
    }

    private async Task LoadTotalByAccount(CancellationToken cancellationToken = default)
    {
        var result = await _accountPresentationService.GetAllTotalByAccountViewModelAsync(cancellationToken);
        if (result.IsSuccess)
        {
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
    private void OnManagePieChart(int valueToAdd)
    {
        IndexOfPositiveNegativeChartValues =
            (IndexOfPositiveNegativeChartValues + valueToAdd + PositiveNegativeChartValues.Length)
            % PositiveNegativeChartValues.Length;

        // var (yearInt, monthInt) = ExtractMonthAndYearFromSelection();
        // UpdatePieChartData(null, monthInt, yearInt);
    }
}