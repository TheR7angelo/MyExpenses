using SQLite;

namespace LibsSql.Tables;

[Table("t_historical")]
public class Historical
{
    [PrimaryKey, AutoIncrement, Column("id")]
    public long Id { get; set; }
    
}