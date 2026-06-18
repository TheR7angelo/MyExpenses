using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Resources.Resx.ExpenseResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.SharedUtils.Resources.Resx.DetailedRecordManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Windows.Dialogs.MsgBox;
using Serilog;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace MyExpenses.Wpf.Pages;

public partial class DashBoardPage
{
    // public ObservableCollection<VHistory> VHistories { get; } = [];
    public ObservableCollection<HistoryViewModel> VHistories { get; } = [];
    // public ObservableCollection<TotalByAccountViewModel> VTotalByAccounts { get; } = [];

    private DataGridRow? DataGridRow { get; set; }

    // public ObservableCollection<CategoryTotal> CategoryTotals { get; } = [];

    // private PieChartManager PieChartManager { get; }

    // private static (bool Positive, bool Negative)[] PositiveNegativeChartValues =>
    // [
    //     (false, true),
    //     (true, true),
    //     (true, false)
    // ];

    // private int IndexOfPositiveNegativeChartValues { get; set; } = 1;

    // public ObservableCollection<string> Years { get; } = [];
    // public ObservableCollection<string> Months { get; } = [];

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

    public static readonly DependencyProperty CurrentVTotalByAccountProperty =
        DependencyProperty.Register(nameof(CurrentVTotalByAccount), typeof(TotalByAccountViewModel), typeof(DashBoardPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(TotalByAccountViewModel)));

    public TotalByAccountViewModel? CurrentVTotalByAccount
    {
        get => (TotalByAccountViewModel)GetValue(CurrentVTotalByAccountProperty);
        set => SetValue(CurrentVTotalByAccountProperty, value);
    }

    // private readonly IExpenseDtoDomainMapper _expenseDtoDomainMapper;
    // private readonly IExpenseDtoViewModelMapper _expenseDtoViewModelMapper;
    private readonly INavigationWindowService _navigationWindowService;

    private readonly IDialogService _dialogService;

    public DashBoardPage(
        // IExpenseDtoDomainMapper expenseDtoDomainMapper,
        // IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
        INavigationWindowService navigationWindowService,
        IDialogService dialogService, DashBoardViewModel dashBoardViewModel)
    {
        // _expenseDtoDomainMapper = expenseDtoDomainMapper;
        // _expenseDtoViewModelMapper = expenseDtoViewModelMapper;
        _navigationWindowService = navigationWindowService;
        _dialogService = dialogService;

        // RefreshAccountTotal();

        InitializeComponent();
        //
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // PieChartManager = new PieChartManager(PieChart, CategoryTotals);
        //
        // UpdateLanguage();
        //
        UpdatePieChartLegendTextPaint();
        //
        // // ReSharper disable HeapView.DelegateAllocation
        // Interface.ThemeChanged += Interface_OnThemeChanged;
        // Interface.LanguageChanged += Interface_OnLanguageChanged;
        // // ReSharper restore HeapView.DelegateAllocation
        //
        // WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, (_, m) =>
        // {
        //     if (m.Value.EntityType is not DependencyType.Account || m.Value.DataAction is not DataAction.Delete) return;
        //
        //     var ids = m.Value.Content;
        //     if (!VTotalByAccounts.Any(s => ids.Contains(s.Id))) return;
        //
        //     foreach (var id in ids)
        //     {
        //         var toRemove = VTotalByAccounts.First(s => s.Id.Equals(id));
        //         VTotalByAccounts.Remove(toRemove);
        //     }
        //
        //     RefreshRadioButtonSelected();
        // });
        //
        // WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountViewModel>>(this, async (_, m) =>
        // {
        //     if (m.Value is not {EntityType: DependencyType.Account, Content: var accountViewModel }) return;
        //
        //     if (m.Value.DataAction is DataAction.Update)
        //     {
        //         var item = VTotalByAccounts.FirstOrDefault(s => s.Id == accountViewModel.Id);
        //         if (item is null) return;
        //         item.Name = accountViewModel.Name ?? string.Empty;
        //         item.Symbol = accountViewModel.CurrencyViewModel?.Symbol ?? string.Empty;
        //     }
        //
        //     if (m.Value.DataAction is DataAction.Add)
        //     {
        //         var result = await accountPresentationService.GetTotalByAccountViewModelAsync(accountViewModel);
        //         if (!result.IsSuccess) return;
        //         VTotalByAccounts!.AddAndSort(result.Value, s => s!.Name);
        //     }
        // });

        DataContext = dashBoardViewModel;
    }

    #region Action

    #region ButtonNavigate

    private void ButtonAccountManagement_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The AccountManagementPage instance is created with the specified DashBoardPage instance to handle account management operations.
        // ShowDialog() is used to open the window modally, pausing the current execution flow until the user closes the dialog.
        // var page = new AccountManagementPage { DashBoardPage = this };
        // nameof(MainWindow.FrameBody).NavigateTo(page);

        nameof(MainWindow.FrameBody).NavigateTo(typeof(AccountManagementPage));
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
        if (button.DataContext is not HistoryViewModel historyViewModel) return;

        EditRecord(historyViewModel);
    }

    private void ButtonPointedRecord_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not VHistory vHistory) return;

        PointRecord(vHistory);
    }


    private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        => DataGridRow = sender as DataGridRow;

    private void Interface_OnThemeChanged()
        => UpdatePieChartLegendTextPaint();

    // private void Interface_OnLanguageChanged()
    // {
    //     UpdateMonthLanguage();
    // }

    // private void ItemsControlVTotalAccount_OnLoaded(object sender, RoutedEventArgs e)
    //     => RefreshRadioButtonSelected();

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

    #endregion

    #region Function

    private void DeleteRecord(VHistory vHistory)
    {
        // TODO correct

        // using var context = new DataBaseContextOld();
        // var history = context.THistories.FirstOrDefault(s => s.Id.Equals(vHistory.Id));
        //
        // var bankTransfer = history?.BankTransferFk is null
        //     ? null
        //     : context.TBankTransfers.FirstOrDefault(s => s.Id == history.BankTransferFk);
        //
        // history?.Delete(true);
        // bankTransfer?.Delete(true);
        //
        // VHistories.Remove(vHistory);
        //
        // var accountName = vHistory.Account!;
        //
        // RefreshDataGrid(accountName);
        // RefreshAccountTotal(CurrentVTotalByAccount!.Id);
    }

    private static void EditRecord(VHistory vHistory)
    {
        // TODO correct

        // var history = vHistory.Id.ToISql<THistory>();
        // if (history is null) return;
        //
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // The RecordExpensePage instance is created with the specified THistory instance to handle record edition operations.
        // // ShowDialog() is used to open the window modally, pausing the current execution flow until the user closes the dialog.
        // var recordExpensePage = new RecordExpensePage();
        // recordExpensePage.SetTHistory(history);
        //
        // nameof(MainWindow.FrameBody).NavigateTo(recordExpensePage);
    }

    private void EditRecord(HistoryViewModel historyViewModel)
    {
        if (historyViewModel.BankTransferViewModel is not null)
        {
            var response = _dialogService.ShowMessageBox(ExpenseResources.MessageBoxUpdateExpenseLindedBankTranferCaption,
                ExpenseResources.MessageBoxUpdateExpenseLindedBankTranferContent,
                Presentation.Enums.MessageBoxButton.YesNoCancel, MsgBoxImage.Question);
            if (response is not Presentation.Enums.MessageBoxResult.Yes) return;
        }

        _navigationWindowService.ManageExpense(historyViewModel);
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

        // RefreshDataGrid();
    }

    // ReSharper disable once HeapView.ClosureAllocation
    // private void RefreshAccountTotal(int id)
    // {
    //     // ReSharper disable once HeapView.ObjectAllocation.Evident
    //     // Necessary instantiation of DataBaseContext to interact with the database.
    //     // This creates a scoped database context for performing queries and modifications in the database.
    //     using var context = new DataBaseContextOld();
    //     var newVTotalByAccount = context.VTotalByAccounts.FirstOrDefault(s => s.Id.Equals(id));
    //     if (newVTotalByAccount is null) return;
    //
    //     // ReSharper disable once HeapView.DelegateAllocation
    //     var vTotalByAccount = VTotalByAccounts.FirstOrDefault(s => s.Id.Equals(id));
    //     if (vTotalByAccount is null) return;
    //
    //     newVTotalByAccount.CopyPropertiesTo(vTotalByAccount);
    // }

    // private async void RefreshAccountTotal()
    // {
    //     // ReSharper disable once HeapView.ClosureAllocation
    //     var newVTotalByAccounts = (await _accountPresentationService.GetAllTotalByAccountViewModelAsync())
    //         .Value!.ToList();
    //
    //     var itemsToDelete = VTotalByAccounts
    //         // ReSharper disable HeapView.DelegateAllocation
    //         // ReSharper disable once HeapView.ClosureAllocation
    //         .Where(s => newVTotalByAccounts.All(n => n.Id != s.Id)).ToArray();
    //     // ReSharper restore HeapView.DelegateAllocation
    //
    //     foreach (var item in itemsToDelete)
    //     {
    //         VTotalByAccounts.Remove(item);
    //     }
    //
    //     // ReSharper disable once HeapView.ClosureAllocation
    //     foreach (var vTotalByAccount in newVTotalByAccounts)
    //     {
    //         // ReSharper disable once HeapView.DelegateAllocation
    //         var exist = VTotalByAccounts.FirstOrDefault(s => s.Id == vTotalByAccount.Id);
    //         if (exist is not null)
    //         {
    //             _accountDtoViewModelMapper.Merge(vTotalByAccount, exist);
    //         }
    //         else
    //         {
    //             VTotalByAccounts.AddAndSort(vTotalByAccount, s => s.Name!);
    //         }
    //     }
    // }

    // private void RefreshDataGrid(string? accountName = null)
    // {
    //     if (string.IsNullOrEmpty(accountName)) accountName = GetAccountName();
    //
    //     if (string.IsNullOrEmpty(accountName)) return;
    //
    //     VHistories.Clear();
    //
    //     var (yearInt, monthInt) = ExtractMonthAndYearFromSelection();
    //     var results = accountName.GetFilteredHistoryDomain(monthInt, yearInt);
    //
    //     var historiesDto = results.Histories.Select(_expenseDtoDomainMapper.MapToDto);
    //     var historiesViewModel = historiesDto.Select(_expenseDtoViewModelMapper.MapToViewModel);
    //
    //     VHistories.AddRange(historiesViewModel);
    //
    //     UpdatePieChartData(accountName, monthInt, yearInt);
    // }

    // private (int? Year, int? Month) ExtractMonthAndYearFromSelection()
    // {
    //     int? monthInt = null;
    //     if (!string.IsNullOrEmpty(SelectedMonth))
    //     {
    //         monthInt = Months.IndexOf(SelectedMonth) + 1;
    //     }
    //
    //     int? yearInt = null;
    //     if (!string.IsNullOrEmpty(SelectedYear))
    //     {
    //         _ = SelectedYear.ToInt(out yearInt);
    //     }
    //
    //     return (yearInt, monthInt);
    // }

    // private void RefreshPositiveNegativeChartValues(int valueToAdd)
    // {
    //     IndexOfPositiveNegativeChartValues =
    //         (IndexOfPositiveNegativeChartValues + valueToAdd + PositiveNegativeChartValues.Length)
    //         % PositiveNegativeChartValues.Length;
    //
    //     var (yearInt, monthInt) = ExtractMonthAndYearFromSelection();
    //     UpdatePieChartData(null, monthInt, yearInt);
    // }

    // private void RefreshRadioButtonSelected()
    // {
    //     var radioButtons = ItemsControlVTotalAccount.FindVisualChildren<RadioButton>().ToList();
    //
    //     var radioButton = StaticVTotalByAccount is null
    //         ? radioButtons.FirstOrDefault()
    //         : radioButtons.FirstOrDefault(rb =>
    //             rb.DataContext is TotalByAccountViewModel vTotalByAccount &&
    //             vTotalByAccount.Id.Equals(StaticVTotalByAccount.Id));
    //
    //     StaticVTotalByAccount = radioButton?.DataContext as TotalByAccountViewModel;
    //
    //     if (radioButton is null) return;
    //     radioButton.IsChecked = true;
    //
    //     RefreshDataGrid();
    //     // RefreshAccountTotal(StaticVTotalByAccount!.Id);
    // }

    // private string? GetAccountName(string? accountName = null)
    // {
    //     if (!string.IsNullOrEmpty(accountName)) return accountName;
    //
    //     var radioButtons = ItemsControlVTotalAccount?.FindVisualChildren<RadioButton>().ToList() ?? [];
    //     if (radioButtons.Count == 0) return null;
    //
    //     return radioButtons.FirstOrDefault(s => s.IsChecked == true)?.Content as string;
    // }

    // private void UpdatePieChartData(string? accountName = null, int? monthInt = null, int? yearInt = null)
    // {
    //     if (string.IsNullOrEmpty(accountName)) accountName = GetAccountName();
    //     if (string.IsNullOrEmpty(accountName)) return;
    //
    //     var filteredData = accountName.GetFilteredVDetailTotalCategories(monthInt, yearInt);
    //
    //     var (positive, negative) = PositiveNegativeChartValues[IndexOfPositiveNegativeChartValues];
    //     var categoriesTotals = filteredData.AggregateCategoryTotalsBySign(out var grandTotal, positive, negative);
    //
    //     PieChartManager.UpdateChartUi(categoriesTotals, grandTotal);
    // }

    private void UpdatePieChartLegendTextPaint()
    {
        var wpfColor = MyExpenses.Wpf.Utils.Resources.GetMaterialDesignBodySkColor();
        PieChart.LegendTextPaint = wpfColor.ToSolidColorPaint();
    }

    #endregion
}