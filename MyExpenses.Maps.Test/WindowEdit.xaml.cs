using MyExpenses.Models.Sql.Tables;
using MyExpenses.Utils;

namespace MyExpenses.Maps.Test;

public partial class WindowEdit
{
    public TPlace TPlace { get; } = new();

    public WindowEdit()
    {
        InitializeComponent();
    }

    public void SetTplace(TPlace newTPlace)
    {
        PropertyCopyHelper.CopyProperties(newTPlace, TPlace);
    }


}