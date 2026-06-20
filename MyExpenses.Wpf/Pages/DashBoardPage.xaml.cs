using System.Windows;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Utils;

namespace MyExpenses.Wpf.Pages;

public partial class DashBoardPage
{
    public DashBoardPage(
        INavigationWindowService navigationWindowService,
        IDialogService dialogService, DashBoardViewModel dashBoardViewModel)
    {
        InitializeComponent();

        UpdatePieChartLegendTextPaint();

        DataContext = dashBoardViewModel;
    }

    #region Action

    #region ButtonNavigate

    private void ButtonAccountManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(AccountManagementPage));

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

    #endregion

    #region Function

    private void UpdatePieChartLegendTextPaint()
    {
        var wpfColor = MyExpenses.Wpf.Utils.Resources.GetMaterialDesignBodySkColor();
        PieChart.LegendTextPaint = wpfColor.ToSolidColorPaint();
    }

    #endregion
}