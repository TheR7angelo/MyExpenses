using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace LibsSql.Tables.DefaultValue;

[Table("t_localisation")]
public class Localisation : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public const string Definition = 
        """
        create table t_localisation
        (
            id         integer
                constraint t_localisation_pk
                    primary key autoincrement,
            name       text,
            number     text,
            street     text,
            postal     integer,
            city       text,
            country    text,
            date_added text default current_date,
            latitude   real,
            longitude  real
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
    
    private string _number = string.Empty;

    [Column("number")]
    public string Number
    {
        get => _number;
        set
        {
            _number = value;
            OnPropertyChanged();
        }
    }
    
    private string _street = string.Empty;

    [Column("street")]
    public string Street
    {
        get => _street;
        set
        {
            _street = value;
            OnPropertyChanged();
        }
    }

    private int _postal;

    [Column("postal")]
    public int Postal
    {
        get => _postal;
        set
        {
            _postal = value;
            OnPropertyChanged();
        }
    }
    
    private string _city = string.Empty;

    [Column("city")]
    public string City
    {
        get => _city;
        set
        {
            _city = value;
            OnPropertyChanged();
        }
    }
    
    private string _country = string.Empty;

    [Column("country")]
    public string Country
    {
        get => _country;
        set
        {
            _country = value;
            OnPropertyChanged();
        }
    }
    
    private DateTime _dateAdded = DateTime.Now;

    [Column("date_added")]
    public DateTime DateAdded
    {
        get => _dateAdded;
        set
        {
            _dateAdded = value;
            OnPropertyChanged();
        }
    }

    private float _latitude;

    [Column("latitude")]
    public float Latitude
    {
        get => _latitude;
        set
        {
            _latitude = value;
            OnPropertyChanged();
        }
    }
    
    private float _longitude;

    [Column("longitude")]
    public float Longitude
    {
        get => _longitude;
        set
        {
            _longitude = value;
            OnPropertyChanged();
        }
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) 
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}