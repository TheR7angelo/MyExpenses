using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AccountManagementContentPage
{
    public static readonly BindableProperty TotalAllAccountProperty = BindableProperty.Create(nameof(TotalAllAccount),
        typeof(double), typeof(AccountManagementContentPage), default(double));

    public double TotalAllAccount
    {
        get => (double)GetValue(TotalAllAccountProperty);
        set => SetValue(TotalAllAccountProperty, value);
    }

    public ObservableCollection<VTotalByAccount> VTotalByAccounts { get; }

    public AccountManagementContentPage()
    {
        using var context = new DataBaseContext();
        VTotalByAccounts = [..context.VTotalByAccounts];
        TotalAllAccount = VTotalByAccounts.Sum(s => s.Total) ?? 0d;

        InitializeComponent();
    }
}