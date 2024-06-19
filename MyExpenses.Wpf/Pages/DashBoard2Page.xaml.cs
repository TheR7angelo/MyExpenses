using System.Windows;
using System.Windows.Input;
using MyExpenses.Wpf.Resources.Resx.Pages.DashBoardPage;

namespace MyExpenses.Wpf.Pages;

public partial class DashBoard2Page
{
    #region Button WrapPanel

    public string ButtonAccountManagement { get; } = DashBoardPageResources.ButtonAccountManagement;
    public string ButtonAccountTypeManagement { get; } = DashBoardPageResources.ButtonAccountTypeManagement;
    public string ButtonCategoryTypeManagement { get; } = DashBoardPageResources.ButtonCategoryTypeManagement;
    public string ButtonColorManagement { get; } = DashBoardPageResources.ButtonColorManagement;
    public string ButtonCurrencyManagement { get; } = DashBoardPageResources.ButtonCurrencyManagement;
    public string ButtonLocationManagement { get; } = DashBoardPageResources.ButtonLocationManagement;
    public string ButtonModePaymentManagement { get; } = DashBoardPageResources.ButtonModePaymentManagement;
    public string ButtonMakeBankTransfer { get; } = DashBoardPageResources.ButtonMakeBankTransfer;
    public string ButtonRecordExpense { get; } = DashBoardPageResources.ButtonRecordExpense;

    #endregion

    public DashBoard2Page()
    {
        InitializeComponent();
    }

    #region Action

    private void ButtonAccountManagement_OnClick(object sender, RoutedEventArgs e)
        => NavigateToAccountManagementPage();
    private void AccountManagementCard_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        => NavigateToAccountManagementPage();

    private void ButtonAccountTypeManagement_OnClick(object sender, RoutedEventArgs e)
        => NavigateToAccountTypeManagementPage();

    private void AccountTypeManagementCard_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        => NavigateToAccountTypeManagementPage();

    private void ButtonBankTransfer_OnClick(object sender, RoutedEventArgs e)
        => NavigateToBankTransferPage();
    private void BankTransferCard_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        => NavigateToBankTransferPage();

    private void ButtonCategoryTypeManagement_OnClick(object sender, RoutedEventArgs e)
        => NavigateToCategoryTypeManagement();

    private void CategoryTypeManagementCard_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        => NavigateToCategoryTypeManagement();

    private void ButtonColorManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(ColorManagementPage));

    private void ButtonCurrencyManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(CurrencyManagementPage));

    private void ButtonFollowUp_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(DashBoardPage));

    private void ButtonLocationManagement_OnClick(object sender, RoutedEventArgs e)
        => NavigateToLocationManagementPage();
    private void LocationManagementCard_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        => NavigateToLocationManagementPage();

    private void ButtonModePaymentManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(ModePaymentManagementPage));

    private void ButtonRecordExpense_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(RecordExpensePage));

    private void LocationManagementUserControl_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        => e.Handled = true;

    #endregion

    #region Function

    private static void NavigateToAccountManagementPage()
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(AccountManagementPage));

    private static void NavigateToAccountTypeManagementPage()
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(AccountTypeManagementPage));

    private static void NavigateToLocationManagementPage()
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(LocationManagementPage));

    private static void NavigateToBankTransferPage()
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(BankTransferPage));

    private static void NavigateToCategoryTypeManagement()
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(CategoryTypeManagementPage));

    #endregion
}