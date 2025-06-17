namespace MyExpenses.Models.Ui;

public class TabItemData
{
    public required string Header { get; init; }

    public required object Content { get; init; }

    public override string ToString() => Header;
}