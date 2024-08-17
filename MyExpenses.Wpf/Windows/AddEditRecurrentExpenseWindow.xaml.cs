using System.Windows;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditRecurrentExpenseWindow
{
    public List<TAccount> Accounts { get; }

    public string SelectedValuePathAccount { get; } = nameof(TAccount.Id);
    public string DisplayMemberPathAccount { get; } = nameof(TAccount.Name);

    public TRecursiveExpense RecursiveExpense { get; set; } = new();
    public AddEditRecurrentExpenseWindow()
    {
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];

        InitializeComponent();
    }

    private void ButtonAccount_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void SetVRecursiveExpense(VRecursiveExpense vRecurrentExpense)
    {
        vRecurrentExpense.CopyPropertiesTo(RecursiveExpense);
    }
}