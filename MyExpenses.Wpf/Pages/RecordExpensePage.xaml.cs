using System.Collections.ObjectModel;
using System.Windows;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Pages;

public partial class RecordExpensePage
{

    public THistory History { get; } = new();

    public string DisplayMemberPathAccount { get; } = nameof(TAccount.Name);
    public string SelectedValuePathAccount { get; } = nameof(TAccount.Id);

    public string DisplayMemberPathCategoryType { get; } = nameof(TCategoryType.Name);
    public string SelectedValuePathCategoryType { get; } = nameof(TCategoryType.Id);
    public string DisplayMemberPathModePayment { get; } = nameof(TModePayment.Name);
    public string SelectedValuePathModePayment { get; } = nameof(TModePayment.Id);

    //TODO work
    public string ComboBoxAccountHintAssist { get; } = "From account :";
    //TODO work
    public string TextBoxDescriptionHintAssist { get; } = "Description :";
    //TODO work
    public string ComboBoxCategoryTypeHintAssist { get; } = "Category type :";
    public string ComboBoxModePaymentHintAssist { get; } = "Mode payment :";

    public required DashBoardPage DashBoardPage { get; set; }

    public ObservableCollection<TAccount> Accounts { get; }
    public ObservableCollection<TCategoryType> CategoryTypes { get; }
    public ObservableCollection<TModePayment> ModePayments { get; }

    public RecordExpensePage()
    {
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];
        CategoryTypes = [..context.TCategoryTypes.OrderBy(s => s.Name)];
        ModePayments = [..context.TModePayments.OrderBy(s => s.Name)];

        InitializeComponent();
    }

    private void ButtonAccount_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
    }

    private void ButtonCategoryType_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
    }

    private void ButtonModePayment_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
    }
}