using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Wpf.Pages;

public partial class AddEditAccountType
{
    public TAccountType AccountType { get; } = new();

    public AddEditAccountType()
    {
        InitializeComponent();
    }
}