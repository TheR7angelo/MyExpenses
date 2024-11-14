using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Objects;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AddEditBankTransferContentPage
{
    public TBankTransfer BankTransfer { get; } = new();
    public TBankTransfer? OriginalBankTransfer { get; private set; }

    public AddEditBankTransferContentPage()
    {
        InitializeComponent();
    }

    public void SetVBankTransferSummary(VBankTransferSummary? vBankTransferSummary)
    {
        if (vBankTransferSummary is null) return;

        var bankTransfer = vBankTransferSummary.Id.ToISql<TBankTransfer>()!;
        bankTransfer.CopyPropertiesTo(BankTransfer);
        OriginalBankTransfer = bankTransfer.DeepCopy();
    }
}