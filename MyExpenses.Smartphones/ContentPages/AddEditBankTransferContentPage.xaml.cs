using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AddEditBankTransferContentPage
{
    public TBankTransfer BankTransfer { get; } = new();

    public AddEditBankTransferContentPage()
    {
        InitializeComponent();
    }
}