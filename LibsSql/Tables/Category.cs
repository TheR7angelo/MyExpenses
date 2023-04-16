using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace LibsSql.Tables;

[Table("t_category")]
public class Category : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public const string Definition =
        """
        CREATE TABLE t_category
        (
            id   integer
                CONSTRAINT t_category_pk
                    PRIMARY KEY AUTOINCREMENT,
            name text
        );
        """;

    private long _id;

    [PrimaryKey, AutoIncrement, Column("id")]
    public long Id
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged();
        }
    }

    private string _name = string.Empty;

    [Column("name")]
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}