using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Maps.Test;

public partial class WindowEdit
{
    public TPlace TPlace { get; init; } = new();

    public WindowEdit()
    {
        InitializeComponent();
    }
}