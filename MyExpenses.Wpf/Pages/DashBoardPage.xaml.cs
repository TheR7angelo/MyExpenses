using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FilterDataGrid;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Queries;
using MyExpenses.Models.Wpf.Charts;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.DashBoardManagement;
using MyExpenses.SharedUtils.Resources.Resx.DetailedRecordManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Queries;
using MyExpenses.Utils;
using MyExpenses.Utils.Strings;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Utils.FilterDataGrid;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class DashBoardPage
{
    public ObservableCollection<VHistory> VHistories { get; } = [];
    public ObservableCollection<VTotalByAccount> VTotalByAccounts { get; } = [];

    private DataGridRow? DataGridRow { get; set; }

    #region Button WrapPanel

    public static readonly DependencyProperty ButtonAccountManagementProperty =
        DependencyProperty.Register(nameof(ButtonAccountManagement), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonAccountManagement
    {
        get => (string)GetValue(ButtonAccountManagementProperty);
        set => SetValue(ButtonAccountManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonAccountTypeManagementProperty =
        DependencyProperty.Register(nameof(ButtonAccountTypeManagement), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonAccountTypeManagement
    {
        get => (string)GetValue(ButtonAccountTypeManagementProperty);
        set => SetValue(ButtonAccountTypeManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonCategoryTypeManagementProperty =
        DependencyProperty.Register(nameof(ButtonCategoryTypeManagement), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonCategoryTypeManagement
    {
        get => (string)GetValue(ButtonCategoryTypeManagementProperty);
        set => SetValue(ButtonCategoryTypeManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonLocationManagementProperty =
        DependencyProperty.Register(nameof(ButtonLocationManagement), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonLocationManagement
    {
        get => (string)GetValue(ButtonLocationManagementProperty);
        set => SetValue(ButtonLocationManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonColorManagementProperty =
        DependencyProperty.Register(nameof(ButtonColorManagement), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonColorManagement
    {
        get => (string)GetValue(ButtonColorManagementProperty);
        set => SetValue(ButtonColorManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonCurrencyManagementProperty =
        DependencyProperty.Register(nameof(ButtonCurrencyManagement), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonCurrencyManagement
    {
        get => (string)GetValue(ButtonCurrencyManagementProperty);
        set => SetValue(ButtonCurrencyManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonModePaymentManagementProperty =
        DependencyProperty.Register(nameof(ButtonModePaymentManagement), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonModePaymentManagement
    {
        get => (string)GetValue(ButtonModePaymentManagementProperty);
        set => SetValue(ButtonModePaymentManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonMakeBankTransferProperty =
        DependencyProperty.Register(nameof(ButtonMakeBankTransfer), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonMakeBankTransfer
    {
        get => (string)GetValue(ButtonMakeBankTransferProperty);
        set => SetValue(ButtonMakeBankTransferProperty, value);
    }

    public static readonly DependencyProperty ButtonRecordExpenseProperty =
        DependencyProperty.Register(nameof(ButtonRecordExpense), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonRecordExpense
    {
        get => (string)GetValue(ButtonRecordExpenseProperty);
        set => SetValue(ButtonRecordExpenseProperty, value);
    }

    public static readonly DependencyProperty ButtonAnalyticsProperty =
        DependencyProperty.Register(nameof(ButtonAnalytics), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonAnalytics
    {
        get => (string)GetValue(ButtonAnalyticsProperty);
        set => SetValue(ButtonAnalyticsProperty, value);
    }

    public static readonly DependencyProperty ButtonRecurrentExpenseProperty =
        DependencyProperty.Register(nameof(ButtonRecurrentExpense), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonRecurrentExpense
    {
        get => (string)GetValue(ButtonRecurrentExpenseProperty);
        set => SetValue(ButtonRecurrentExpenseProperty, value);
    }

    #endregion

    #region DataGrid

    public static readonly DependencyProperty DataGridMenuItemHeaderEditRecordProperty =
        DependencyProperty.Register(nameof(DataGridMenuItemHeaderEditRecord), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string DataGridMenuItemHeaderEditRecord
    {
        get => (string)GetValue(DataGridMenuItemHeaderEditRecordProperty);
        set => SetValue(DataGridMenuItemHeaderEditRecordProperty, value);
    }

    public static readonly DependencyProperty DataGridMenuItemHeaderDeleteRecordProperty =
        DependencyProperty.Register(nameof(DataGridMenuItemHeaderDeleteRecord), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string DataGridMenuItemHeaderDeleteRecord
    {
        get => (string)GetValue(DataGridMenuItemHeaderDeleteRecordProperty);
        set => SetValue(DataGridMenuItemHeaderDeleteRecordProperty, value);
    }

    public static readonly DependencyProperty DataGridCheckBoxColumnPointedProperty =
        DependencyProperty.Register(nameof(DataGridCheckBoxColumnPointed), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string DataGridCheckBoxColumnPointed
    {
        get => (string)GetValue(DataGridCheckBoxColumnPointedProperty);
        set => SetValue(DataGridCheckBoxColumnPointedProperty, value);
    }

    public static readonly DependencyProperty ButtonContentEditRecordProperty =
        DependencyProperty.Register(nameof(ButtonContentEditRecord), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonContentEditRecord
    {
        get => (string)GetValue(ButtonContentEditRecordProperty);
        set => SetValue(ButtonContentEditRecordProperty, value);
    }

    public static readonly DependencyProperty ButtonContentDeleteRecordProperty =
        DependencyProperty.Register(nameof(ButtonContentDeleteRecord), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonContentDeleteRecord
    {
        get => (string)GetValue(ButtonContentDeleteRecordProperty);
        set => SetValue(ButtonContentDeleteRecordProperty, value);
    }

    public static readonly DependencyProperty ButtonContentPointedRecordProperty =
        DependencyProperty.Register(nameof(ButtonContentPointedRecord), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonContentPointedRecord
    {
        get => (string)GetValue(ButtonContentPointedRecordProperty);
        set => SetValue(ButtonContentPointedRecordProperty, value);
    }

    #endregion

    public static readonly DependencyProperty ComboBoxYearsHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxYearsHintAssist), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ComboBoxYearsHintAssist
    {
        get => (string)GetValue(ComboBoxYearsHintAssistProperty);
        set => SetValue(ComboBoxYearsHintAssistProperty, value);
    }

    public static readonly DependencyProperty ComboBoxMonthHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxMonthHintAssist), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ComboBoxMonthHintAssist
    {
        get => (string)GetValue(ComboBoxMonthHintAssistProperty);
        set => SetValue(ComboBoxMonthHintAssistProperty, value);
    }

    public ObservableCollection<CategoryTotal> CategoryTotals { get; } = [];

    private PieChartManager PieChartManager { get; }

    public ObservableCollection<string> Years { get; }
    public ObservableCollection<string> Months { get; } = [];

    public static readonly DependencyProperty SelectedYearProperty = DependencyProperty.Register(nameof(SelectedYear),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(string), typeof(DashBoardPage), new PropertyMetadata(default(string)));

    public string SelectedYear
    {
        get => (string)GetValue(SelectedYearProperty);
        set => SetValue(SelectedYearProperty, value);
    }

    public static readonly DependencyProperty SelectedMonthProperty = DependencyProperty.Register(nameof(SelectedMonth),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(string), typeof(DashBoardPage), new PropertyMetadata(default(string)));

    public string SelectedMonth
    {
        get => (string)GetValue(SelectedMonthProperty);
        set => SetValue(SelectedMonthProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty LocalLanguageProperty = DependencyProperty.Register(nameof(LocalLanguage),
        typeof(Local), typeof(DashBoardPage), new PropertyMetadata(default(Local)));

    public Local LocalLanguage
    {
        get => (Local)GetValue(LocalLanguageProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(LocalLanguageProperty, value);
    }

    public static readonly DependencyProperty DateFormatStringProperty =
        DependencyProperty.Register(nameof(DateFormatString), typeof(string), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string DateFormatString
    {
        get => (string)GetValue(DateFormatStringProperty);
        set => SetValue(DateFormatStringProperty, value);
    }

    public static readonly DependencyProperty CurrentVTotalByAccountProperty =
        DependencyProperty.Register(nameof(CurrentVTotalByAccount), typeof(VTotalByAccount), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(VTotalByAccount)));

    public VTotalByAccount? CurrentVTotalByAccount
    {
        get => (VTotalByAccount)GetValue(CurrentVTotalByAccountProperty);
        set => SetValue(CurrentVTotalByAccountProperty, value);
    }

    private static DashBoardPage Instance { get; set; } = null!;

    private static VTotalByAccount? _staticVTotalByAccount;
    private static VTotalByAccount? StaticVTotalByAccount
    {
        get => _staticVTotalByAccount;
        set
        {
            _staticVTotalByAccount = value;
            Instance.CurrentVTotalByAccount = value;
        }
    }

    public DashBoardPage()
    {
        Instance = this;

        var (currentYear, currentMonth, _) = DateTime.Now;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        var recurrences = context.GetActiveRecurrencesForCurrentMonth(currentYear, currentMonth);

        if (recurrences.Any())
        {
            var mainWindow = Application.Current.MainWindow!;
            var actualWidth = mainWindow.ActualWidth;
            var actualHeight = mainWindow.ActualHeight;
            var size = new Size(actualWidth, actualHeight);

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The RecurrentAddWindow instance is created with the specified size to handle recurrent addition operations.
            // ShowDialog() is used to open the window modally, pausing the current execution flow until the user closes the dialog.
            var recurrentAddWindow = new RecurrentAddWindow(size);
            recurrentAddWindow.ShowDialog();
        }

        UpdateMonthLanguage();

        Years =
        [
            ..context.GetDistinctYearsFromHistories(SortOrder.Descending)
                .Select(s => s.ToString())
        ];

        if (Years.Count.Equals(0)) Years.Add(currentYear.ToString());
        var lastYear = int.Parse(Years.Max()!);
        for (var year = lastYear + 1; year <= currentYear; year++)
        {
            Years.Insert(0, year.ToString());
        }

        SelectedYear = currentYear.ToString();
        SelectedMonth = Months[currentMonth - 1];

        RefreshAccountTotal();

        InitializeComponent();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        PieChartManager = new PieChartManager(PieChart, CategoryTotals);

        UpdateLanguage();

        UpdatePieChartLegendTextPaint();

        // ReSharper disable HeapView.DelegateAllocation
        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
        // ReSharper restore HeapView.DelegateAllocation
    }

    #region Action

    #region ButtonNavigate

    private void ButtonAccountManagement_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The AccountManagementPage instance is created with the specified DashBoardPage instance to handle account management operations.
        // ShowDialog() is used to open the window modally, pausing the current execution flow until the user closes the dialog.
        var page = new AccountManagementPage { DashBoardPage = this };
        nameof(MainWindow.FrameBody).NavigateTo(page);
    }

    private void ButtonAccountTypeManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(AccountTypeManagementPage));

    private void ButtonAnalytics_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(AnalyticsPage));

    private void ButtonCategoryTypeManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(CategoryTypeManagementPage));

    private void ButtonColorManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(ColorManagementPage));

    private void ButtonCurrencyManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(CurrencyManagementPage));

    private void ButtonLocationManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(LocationManagementPage));

    private void ButtonModePaymentManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(ModePaymentManagementPage));

    private void ButtonMakeBankTransfer_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(BankTransferPage));

    private void ButtonRecordExpense_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(RecordExpensePage));

    private void ButtonRecurrentExpense_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(RecurrentExpensePage));

    #endregion

    private void ButtonAddMonth_OnClick(object sender, RoutedEventArgs e)
    {
        var date = GetDateOnlyFilter();
        date = date.AddMonths(1);

        var result = UpdateFilterDate(date);

        if (result) return;

        MsgBox.Show(DashBoardManagementResources.MessageBoxAddMonthErrorTitle,
            DashBoardManagementResources.MessageBoxAddMonthErrorMessage, MessageBoxButton.OK, MsgBoxImage.Warning);
    }

    private void ButtonDateNow_OnClick(object sender, RoutedEventArgs e)
    {
        var now = DateOnly.FromDateTime(DateTime.Now);
        UpdateFilterDate(now);
    }

    private void ButtonDeleteRecord_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.Show(
            DetailedRecordManagementResources.MessageBoxDeleteHistoryQuestionTitle,
            DetailedRecordManagementResources.MessageBoxDeleteHistoryQuestionMessage,
            MessageBoxButton.YesNoCancel, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) return;

        var button = (Button)sender;
        if (button.DataContext is not VHistory vHistory) return;

        DeleteRecord(vHistory);
    }

    private void ButtonEditRecord_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not VHistory vHistory) return;

        EditRecord(vHistory);
    }

    private void ButtonPointedRecord_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not VHistory vHistory) return;

        PointRecord(vHistory);
    }

    private void ButtonRemoveMonth_OnClick(object sender, RoutedEventArgs e)
    {
        var date = GetDateOnlyFilter();
        date = date.AddMonths(-1);

        var result = UpdateFilterDate(date);

        if (result) return;

        MsgBox.Show(DashBoardManagementResources.MessageBoxRemoveMonthErrorTitle,
            DashBoardManagementResources.MessageBoxRemoveMonthErrorMessage, MessageBoxButton.OK, MsgBoxImage.Warning);
    }

    private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        => DataGridRow = sender as DataGridRow;

    private void Interface_OnThemeChanged()
        => UpdatePieChartLegendTextPaint();

    private void Interface_OnLanguageChanged()
    {
        UpdateLanguage();
        UpdateMonthLanguage();
    }

    private void ItemsControlVTotalAccount_OnLoaded(object sender, RoutedEventArgs e)
        => RefreshRadioButtonSelected();

    private void MenuItemDeleteRecord_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.Show(
            DetailedRecordManagementResources.MessageBoxDeleteHistoryQuestionTitle,
            DetailedRecordManagementResources.MessageBoxDeleteHistoryQuestionMessage,
            MessageBoxButton.YesNoCancel, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) return;

        if (DataGridRow!.DataContext is not VHistory vHistory) return;

        DeleteRecord(vHistory);
    }

    private void MenuItemEditRecord_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGridRow!.DataContext is not VHistory vHistory) return;

        EditRecord(vHistory);
    }

    private void MenuItemPointed_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGridRow!.DataContext is not VHistory vHistory) return;
        PointRecord(vHistory);
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => RefreshDataGrid();

    private void ToggleButtonVTotalAccount_OnChecked(object sender, RoutedEventArgs e)
    {
        var button = (RadioButton)sender;
        if (button.DataContext is not VTotalByAccount vTotalByAccount) return;

        StaticVTotalByAccount = vTotalByAccount;

        // Total = vTotalByAccount.Total;
        // Symbol = vTotalByAccount.Symbol;

        var name = vTotalByAccount.Name;
        if (string.IsNullOrEmpty(name)) return;

        RefreshDataGrid(name);
    }

    #endregion

    #region Function

    private void DeleteRecord(VHistory vHistory)
    {
        var history = vHistory.Id.ToISql<THistory>();

        history?.Delete(true);

        VHistories.Remove(vHistory);

        var accountName = vHistory.Account!;

        RefreshDataGrid(accountName);
        RefreshAccountTotal(CurrentVTotalByAccount!.Id);
    }

    private static void EditRecord(VHistory vHistory)
    {
        var history = vHistory.Id.ToISql<THistory>();
        if (history is null) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The RecordExpensePage instance is created with the specified THistory instance to handle record edition operations.
        // ShowDialog() is used to open the window modally, pausing the current execution flow until the user closes the dialog.
        var recordExpensePage = new RecordExpensePage();
        recordExpensePage.SetTHistory(history);

        nameof(MainWindow.FrameBody).NavigateTo(recordExpensePage);
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

    private void PointRecord(VHistory vHistory)
    {
        var history = vHistory.Id.ToISql<THistory>()!;

        history.IsPointed = !history.IsPointed;

        if (history.IsPointed) history.DatePointed = DateTime.Now;
        else history.DatePointed = null;

        Log.Information("Attention to pointed record, id: \"{HistoryId}\"", history.Id);
        history.AddOrEdit();
        Log.Information("The recording was successfully pointed");

        RefreshDataGrid();
    }

    // ReSharper disable once HeapView.ClosureAllocation
    private void RefreshAccountTotal(int id)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        var newVTotalByAccount = context.VTotalByAccounts.FirstOrDefault(s => s.Id.Equals(id));
        if (newVTotalByAccount is null) return;

        // ReSharper disable once HeapView.DelegateAllocation
        var vTotalByAccount = VTotalByAccounts.FirstOrDefault(s => s.Id.Equals(id));
        if (vTotalByAccount is null) return;

        newVTotalByAccount.CopyPropertiesTo(vTotalByAccount);
    }

    internal void RefreshAccountTotal()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();

        // ReSharper disable once HeapView.ClosureAllocation
        var newVTotalByAccounts = context.VTotalByAccounts.ToList();

        var itemsToDelete = VTotalByAccounts
            // ReSharper disable HeapView.DelegateAllocation
            // ReSharper disable once HeapView.ClosureAllocation
            .Where(s => newVTotalByAccounts.All(n => n.Id != s.Id)).ToArray();
        // ReSharper restore HeapView.DelegateAllocation

        foreach (var item in itemsToDelete)
        {
            VTotalByAccounts.Remove(item);
        }

        // ReSharper disable once HeapView.ClosureAllocation
        foreach (var vTotalByAccount in newVTotalByAccounts)
        {
            // ReSharper disable once HeapView.DelegateAllocation
            var exist = VTotalByAccounts.FirstOrDefault(s => s.Id == vTotalByAccount.Id);
            if (exist is not null)
            {
                vTotalByAccount.CopyPropertiesTo(exist);
            }
            else
            {
                VTotalByAccounts.AddAndSort(vTotalByAccount, s => s.Name!);
            }
        }
    }

    private void RefreshDataGrid(string? accountName = null)
    {
        if (string.IsNullOrEmpty(accountName)) accountName = GetAccountName();

        if (string.IsNullOrEmpty(accountName)) return;

        VHistories.Clear();

        int? monthInt = null;
        if (!string.IsNullOrEmpty(SelectedMonth))
        {
            monthInt = Months.IndexOf(SelectedMonth) + 1;
        }

        int? yearInt = null;
        if (!string.IsNullOrEmpty(SelectedYear))
        {
            _ = SelectedYear.ToInt(out yearInt);
        }

        var results = accountName.GetFilteredHistories(monthInt, yearInt);
        VHistories.AddRange(results.Histories);

        var filteredData = accountName.GetFilteredVDetailTotalCategories(monthInt, yearInt);
        var categoriesTotals = filteredData.CalculateCategoryTotals(out var grandTotal);

        PieChartManager.UpdateChartUi(categoriesTotals, grandTotal);
    }

    private void RefreshRadioButtonSelected()
    {
        var radioButtons = ItemsControlVTotalAccount.FindVisualChildren<RadioButton>().ToList();

        var radioButton = StaticVTotalByAccount is null
            ? radioButtons.FirstOrDefault()
            : radioButtons.FirstOrDefault(rb =>
                rb.DataContext is VTotalByAccount vTotalByAccount &&
                vTotalByAccount.Id.Equals(StaticVTotalByAccount.Id));

        StaticVTotalByAccount = radioButton?.DataContext as VTotalByAccount;

        if (radioButton is null) return;
        radioButton.IsChecked = true;

        RefreshDataGrid();
        RefreshAccountTotal(StaticVTotalByAccount!.Id);
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

    private string? GetAccountName(string? accountName = null)
    {
        if (!string.IsNullOrEmpty(accountName)) return accountName;

        var radioButtons = ItemsControlVTotalAccount?.FindVisualChildren<RadioButton>().ToList() ?? [];
        if (radioButtons.Count == 0) return null;

        return radioButtons.FirstOrDefault(s => s.IsChecked == true)?.Content as string;
    }

    private void UpdateMonthLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;
        LocalLanguage = currentCulture.ToLocal();

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

    private void UpdateLanguage()
    {
        ButtonAccountManagement = DashBoardManagementResources.ButtonAccountManagement;
        ButtonAccountTypeManagement = DashBoardManagementResources.ButtonAccountTypeManagement;
        ButtonCategoryTypeManagement = DashBoardManagementResources.ButtonCategoryTypeManagement;
        ButtonColorManagement = DashBoardManagementResources.ButtonColorManagement;
        ButtonCurrencyManagement = DashBoardManagementResources.ButtonCurrencyManagement;
        ButtonLocationManagement = DashBoardManagementResources.ButtonLocationManagement;
        ButtonModePaymentManagement = DashBoardManagementResources.ButtonModePaymentManagement;
        ButtonMakeBankTransfer = DashBoardManagementResources.ButtonMakeBankTransfer;
        ButtonRecordExpense = DashBoardManagementResources.ButtonRecordExpense;
        ButtonAnalytics = DashBoardManagementResources.ButtonAnalytics;
        ButtonRecurrentExpense = DashBoardManagementResources.ButtonRecurrentExpense;

        TextColumnAccount.Header = DashBoardManagementResources.DataGridTextColumnAccount;
        TextColumnDescription.Header = DashBoardManagementResources.DataGridTextColumnDescription;
        TemplateColumnCategory.Header = DashBoardManagementResources.DataGridTextColumnCategory;
        TextColumnModePayment.Header = DashBoardManagementResources.DataGridTextColumnModePayment;
        TemplateColumnValue.Header = DashBoardManagementResources.DataGridTextColumnValue;
        TextColumnDate.Header = DashBoardManagementResources.DataGridTextColumnDate;
        TextColumnPlace.Header = DashBoardManagementResources.DataGridTextColumnPlace;
        CheckBoxColumnPointed.Header = DashBoardManagementResources.DataGridTextColumnPointed;
        ButtonContentPointedRecord = DashBoardManagementResources.DataGridTextColumnPointed;

        TemplateColumnActions.Header = DashBoardManagementResources.DataGridTemplateColumnActionsHeader;
        ButtonContentPointedRecord = DashBoardManagementResources.DataGridTextColumnPointed;
        ButtonContentEditRecord = DashBoardManagementResources.ButtonContentEditRecord;
        ButtonContentDeleteRecord = DashBoardManagementResources.ButtonContentDeleteRecord;

        DataGridCheckBoxColumnPointed = DashBoardManagementResources.DataGridTextColumnPointed;
        DataGridMenuItemHeaderEditRecord = DashBoardManagementResources.ButtonContentEditRecord;
        DataGridMenuItemHeaderDeleteRecord = DashBoardManagementResources.ButtonContentDeleteRecord;

        ComboBoxYearsHintAssist = DashBoardManagementResources.ComboBoxYearsHintAssist;
        ComboBoxMonthHintAssist = DashBoardManagementResources.ComboBoxMonthHintAssist;

        DateFormatString = SharedUtils.Converters.DateTimeToDateTimeWithoutSecondsConverter.GetDateTimePattern();
    }

    private void UpdatePieChartLegendTextPaint()
    {
        var wpfColor = MyExpenses.Wpf.Utils.Resources.GetMaterialDesignBodySkColor();
        PieChart.LegendTextPaint = wpfColor.ToSolidColorPaint();
    }

    #endregion
}