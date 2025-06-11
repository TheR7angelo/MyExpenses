using System.Collections.ObjectModel;
using System.Globalization;
using MyExpenses.Maui.Utils;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Queries;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources.Resx.DashBoardManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Queries;
using MyExpenses.Utils.Strings;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AccountAnalyzedByMonthContentPage
{
    public static readonly BindableProperty ComboBoxMonthHintAssistProperty = BindableProperty.Create(nameof(ComboBoxMonthHintAssist), typeof(string), typeof(AccountAnalyzedByMonthContentPage), default(string));
    public static readonly BindableProperty ComboBoxYearsHintAssistProperty = BindableProperty.Create(nameof(ComboBoxYearsHintAssist), typeof(string), typeof(AccountAnalyzedByMonthContentPage), default(string));
    public ObservableCollection<string> Years { get; }
    public ObservableCollection<string> Months { get; } = [];
    public ObservableCollection<TAccount> Accounts { get; } = [];

    public static readonly BindableProperty SelectedYearProperty = BindableProperty.Create(nameof(SelectedYear),
        typeof(string), typeof(DashBoardContentPage));

    public string SelectedYear
    {
        get => (string)GetValue(SelectedYearProperty);
        set => SetValue(SelectedYearProperty, value);
    }

    public static readonly BindableProperty SelectedMonthProperty = BindableProperty.Create(nameof(SelectedMonth),
        typeof(string), typeof(DashBoardContentPage));

    public string SelectedMonth
    {
        get => (string)GetValue(SelectedMonthProperty);
        set => SetValue(SelectedMonthProperty, value);
    }

    public string ComboBoxYearsHintAssist
    {
        get => (string)GetValue(ComboBoxYearsHintAssistProperty);
        set => SetValue(ComboBoxYearsHintAssistProperty, value);
    }

    public string ComboBoxMonthHintAssist
    {
        get => (string)GetValue(ComboBoxMonthHintAssistProperty);
        set => SetValue(ComboBoxMonthHintAssistProperty, value);
    }

    public AccountAnalyzedByMonthContentPage()
    {
        UpdateMonthLanguage();

        var (currentYear, currentMonth, _) = DateTime.Now;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The use of `using var context = new DataBaseContext();` is essential here because
        // we need to interact with the database to retrieve the required data.
        // The DataBaseContext instance provides access to execute SQL queries or LINQ
        // operations on the database tables and views. Using the `using` statement ensures
        // proper disposal of resources (like database connections) once the context is no
        // longer needed, optimizing resource management and preventing potential memory leaks.
        using var context = new DataBaseContext();
        Years =
        [
            ..context.GetDistinctYearsFromHistories(SortOrder.Descending)
                .Select(y => y.ToString())
        ];

        if (Years.Count.Equals(0)) Years.Add(currentYear.ToString());
        var lastYear = int.Parse(Years.Max()!);
        for (var year = lastYear + 1; year <= currentYear; year++)
        {
            Years.Insert(0, year.ToString());
        }

        SelectedYear = currentYear.ToString();
        SelectedMonth = Months[currentMonth - 1];

        Accounts.AddRangeAndSort(context.TAccounts, s => s.Name!);

        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged()
    {
        UpdateMonthLanguage();
        UpdateLanguage();
    }

    private void UpdateLanguage()
    {
        ComboBoxYearsHintAssist = DashBoardManagementResources.ComboBoxYearsHintAssist;
        ComboBoxMonthHintAssist = DashBoardManagementResources.ComboBoxMonthHintAssist;
    }

    private void UpdateMonthLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;

        var months = currentCulture.DateTimeFormat.MonthNames
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(s => s.ToFirstCharUpper()).ToList();

        if (Months.Count is 0)
        {
            Months.AddRange(months);
        }
        else
        {
            // ReSharper disable once HeapView.DelegateAllocation
            var selectedMonth = Months.FirstOrDefault(month => month.Equals(SelectedMonth)) ?? string.Empty;
            for (var i = 0; i < months.Count; i++)
            {
                Months[i] = months[i];
            }

            SelectedMonth = selectedMonth;
        }
    }

    private void ButtonAddMonth_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonAddMonth();

    private void ButtonDateNow_OnClick(object? sender, EventArgs e)
    {
        var now = DateOnly.FromDateTime(DateTime.Now);
        UpdateFilterDate(now);
    }

    private void ButtonRemoveMonth_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonRemoveMonth();

    private async Task HandleButtonAddMonth()
    {
        var date = GetDateOnlyFilter();
        date = date.AddMonths(1);

        var result = UpdateFilterDate(date);

        if (result) return;

        await DisplayAlert(DashBoardManagementResources.MessageBoxAddMonthErrorTitle,
            DashBoardManagementResources.MessageBoxAddMonthErrorMessage,
            DashBoardManagementResources.MessageBoxAddMonthErrorOkButton);
    }

    private DateOnly GetDateOnlyFilter()
    {
        var monthIndex = string.IsNullOrEmpty(SelectedMonth)
            ? DateTime.Now.Month
            : Months.IndexOf(SelectedMonth) + 1;

        var year = string.IsNullOrEmpty(SelectedYear)
            ? DateTime.Now.Year
            : int.Parse(SelectedYear);

        var date = DateOnly.Parse($"{year}/{monthIndex}/01");
        return date;
    }

    private bool UpdateFilterDate(DateOnly date)
    {
        var yearStr = date.Year.ToString();
        if (!Years.Contains(yearStr)) return false;

        if (!yearStr.Equals(SelectedYear)) SelectedYear = yearStr;

        var monthIndex = date.Month - 1;
        SelectedMonth = Months[monthIndex];

        return true;
    }

    private async Task HandleButtonRemoveMonth()
    {
        var date = GetDateOnlyFilter();
        date = date.AddMonths(-1);

        var result = UpdateFilterDate(date);

        if (result) return;

        await DisplayAlert(DashBoardManagementResources.MessageBoxRemoveMonthErrorTitle,
            DashBoardManagementResources.MessageBoxRemoveMonthErrorMessage,
            DashBoardManagementResources.MessageBoxRemoveMonthErrorOkButton);
    }

    private void CustomPicker_OnSelectedIndexChanged(object? sender, EventArgs e)
    {

    }

    private void RefreshList()
    {
        var radioButtons = CollectionViewAccount?.FindVisualChildren<RadioButton>().ToList() ?? [];
        if (radioButtons.Count.Equals(0)) return;

        var accountName = radioButtons.FirstOrDefault(s => s.IsChecked)?.Content as string;

        if (string.IsNullOrEmpty(accountName)) return;

        // var rowTotalCount = FilterAndSortHistoryRecords(accountName, out var records, out var elapsedTimeLoadingData);
        //
        // VHistories.Clear();
        // VHistories.AddRange(records);
    }

    private void CollectionAccount_OnLoaded(object? sender, EventArgs e)
    {
        _ = Dispatcher.DispatchAsync(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            RefreshRadioButtonSelected();
        });
    }

    private void RefreshRadioButtonSelected()
    {
        var radioButtons = CollectionViewAccount.FindVisualChildren<RadioButton>().ToList();
        if (radioButtons.Count is 0) return;

        var radioButton = radioButtons.FirstOrDefault(s => s.IsChecked);
        if (radioButton is null)
        {
            radioButton = radioButtons.First();
            radioButton.IsChecked = true;
        }

        RefreshList();
    }

    private void RadioButton_OnCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {

    }
}