using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace LibsSql.Tables;

[Table("t_wallet")]
public class Wallet : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public const string Definition =
        """
        CREATE TABLE t_wallet
        (
            id        integer
                CONSTRAINT t_wallet_pk
                    PRIMARY KEY AUTOINCREMENT,
            name      text,
            wallet_fk integer
                CONSTRAINT t_wallet_t_wallet_type_id_fk
                    REFERENCES t_wallet_type
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
    public string Name { 
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    private long _walletType;
    
    [ForeignKey(typeof(WalletType)), Column("wallet_fk")]
    public long WalletType 
    { 
        get => _walletType;
        set
        {
            _walletType = value;
            OnPropertyChanged();
        }
        
    }

    private bool _external;

    [Column("external")]
    public bool External
    {
        get => _external;
        set
        {
            _external = value;
            OnPropertyChanged();
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}