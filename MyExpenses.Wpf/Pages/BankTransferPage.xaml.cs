using System.Windows;
using MyExpenses.Presentation.ViewModel;

namespace MyExpenses.Wpf.Pages;

public partial class BankTransferPage
{
    public BankTransferPage(BankTransferManagementViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadCommand.ExecuteAsync(null);
    }

    private void ButtonValidBankTransferPreview_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO: Implement bank transfer validation
    }
}