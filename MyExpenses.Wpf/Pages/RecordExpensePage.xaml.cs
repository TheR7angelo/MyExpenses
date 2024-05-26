using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Pages;

public partial class RecordExpensePage
{

    public THistory History { get; } = new();

    public string DisplayMemberPathAccount { get; } = nameof(TAccount.Name);
    public string SelectedValuePathAccount { get; } = nameof(TAccount.Id);

    //TODO work
    public string ComboBoxAccountHintAssist { get; } = "From account :";

    public required DashBoardPage DashBoardPage { get; set; }

    public ObservableCollection<TAccount> Accounts { get; }

    public RecordExpensePage()
    {
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];

        InitializeComponent();
    }

}