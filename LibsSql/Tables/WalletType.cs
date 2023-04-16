using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace LibsSql.Tables;

[Table("t_wallet_type")]
public class WalletType : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public const string Definition =
        """
        CREATE TABLE t_wallet_type
        (
            id   integer NOT NULL
                PRIMARY KEY AUTOINCREMENT,
            name TEXT
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