namespace MyExpenses.Models.Sql.Tables;

public partial class TCategoryType
{
    public TCategoryType DeepCopy()
    {
        return new TCategoryType
        {
            Id = Id,
            Name = Name,
            ColorFk = ColorFk,
            DateAdded = DateAdded
        };
    }
}