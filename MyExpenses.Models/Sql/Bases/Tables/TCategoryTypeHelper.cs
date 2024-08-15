namespace MyExpenses.Models.Sql.Bases.Tables;

public partial class TCategoryType
{
    public Bases.Tables.TCategoryType DeepCopy()
    {
        return new Bases.Tables.TCategoryType
        {
            Id = Id,
            Name = Name,
            ColorFk = ColorFk,
            DateAdded = DateAdded
        };
    }
}