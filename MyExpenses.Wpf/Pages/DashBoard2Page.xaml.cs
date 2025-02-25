using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.SharedUtils.Resources.Resx.DashBoardManagement;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Pages;

public partial class DashBoard2Page
{
    #region Button WrapPanel

    public string ButtonAccountManagement { get; } = DashBoardManagementResources.ButtonAccountManagement;
    public string ButtonAccountTypeManagement { get; } = DashBoardManagementResources.ButtonAccountTypeManagement;
    public string ButtonCategoryTypeManagement { get; } = DashBoardManagementResources.ButtonCategoryTypeManagement;
    public string ButtonColorManagement { get; } = DashBoardManagementResources.ButtonColorManagement;
    public string ButtonCurrencyManagement { get; } = DashBoardManagementResources.ButtonCurrencyManagement;
    public string ButtonLocationManagement { get; } = DashBoardManagementResources.ButtonLocationManagement;
    public string ButtonModePaymentManagement { get; } = DashBoardManagementResources.ButtonModePaymentManagement;
    public string ButtonMakeBankTransfer { get; } = DashBoardManagementResources.ButtonMakeBankTransfer;
    public string ButtonRecordExpense { get; } = DashBoardManagementResources.ButtonRecordExpense;
    // TODO work
    public string TextBoxSearchHintAssist { get; } = "Search here";

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
        => NavigateToColorManagement();
    private void ColorManagementCard_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        => NavigateToColorManagement();

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

    private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var keyword = textBox.Text.Trim();

        var cards = StackPanelCards.Children
            .OfType<MaterialDesignThemes.Wpf.Card>().ToList();

        if (string.IsNullOrWhiteSpace(keyword))
        {
            foreach (var expander in cards.SelectMany(card => card.FindVisualChildren<Expander>()))
            {
                expander.IsExpanded = false;

                foreach (var button in expander.FindVisualChildren<Button>())
                {
                    button.Visibility = Visibility.Visible;
                }

                if (expander.Content is not ScrollViewer scrollViewer) continue;
                if (scrollViewer.Content is not StackPanel stackPanel) continue;

                foreach (var button in stackPanel.Children.OfType<Button>())
                {
                    button.Visibility = Visibility.Visible;
                }
            }
        }
        else
        {
            foreach (var expander in cards.SelectMany(card => card.FindVisualChildren<Expander>()))
            {
                var containsKeyword = false;

                if (expander.Content is not ScrollViewer scrollViewer) continue;
                if (scrollViewer.Content is not StackPanel stackPanel) continue;

                foreach (var button in stackPanel.Children.OfType<Button>())
                {
                    var contentStr = button.Content.ToString() ?? string.Empty;
                    if (contentStr.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        button.Visibility = Visibility.Visible;
                        containsKeyword = true;
                    }
                    else
                    {
                        button.Visibility = Visibility.Collapsed;
                    }
                }

                expander.IsExpanded = containsKeyword;
            }
        }
    }

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

    private static void NavigateToColorManagement()
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(ColorManagementPage));

    #endregion
}