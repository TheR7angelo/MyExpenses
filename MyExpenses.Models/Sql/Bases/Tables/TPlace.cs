using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyExpenses.Models.IO.Sig.Interfaces;
using NetTopologySuite.Geometries;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Tables;

[AddINotifyPropertyChangedInterface]
[Table("t_place")]
public partial class TPlace : ISql, ISig
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("number")]
    public string? Number { get; set; }

    [Column("street")]
    public string? Street { get; set; }

    [Column("postal")]
    public string? Postal { get; set; }

    [Column("city")]
    public string? City { get; set; }

    [Column("country")]
    public string? Country { get; set; }

    [NotMapped]
    private double? _latitude;

    [Column("latitude")]
    public double? Latitude
    {
        get => _latitude;
        set
        {
            _latitude = value;
            UpdateGeometry();
        }
    }

    [NotMapped]
    private double? _longitude;

    [Column("longitude")]
    public double? Longitude
    {
        get => _longitude;
        set
        {
            _longitude = value;
            UpdateGeometry();
        }

    }

    [NotMapped]
    private Geometry? _geometry;

    [NotMapped]
    public Geometry? Geometry
    {
        get => _geometry;
        set
        {
            _geometry = value;
            if (_geometry is null)
            {
                _longitude = null;
                _latitude = null;
            }
            else
            {
                var point = (Point)_geometry;
                _longitude = point.X;
                _latitude = point.Y;
            }
        }
    }

    [Column("is_open", TypeName = "BOOLEAN")]
    public bool? IsOpen { get; set; } = true;

    [Column("can_be_deleted", TypeName = "BOOLEAN")]
    public bool? CanBeDeleted { get; set; } = true;

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;

    [InverseProperty("PlaceFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();

    [InverseProperty("PlaceFkNavigation")]
    public virtual ICollection<TRecursiveExpense> TRecursiveExpenses { get; set; } = new List<TRecursiveExpense>();

    public override string ToString()
    {
        var partAddress = new List<string>();
        if (!string.IsNullOrEmpty(Number)) partAddress.Add(Number);
        if (!string.IsNullOrEmpty(Street)) partAddress.Add(Street);
        if (!string.IsNullOrEmpty(Postal)) partAddress.Add(Postal);
        if (!string.IsNullOrEmpty(City)) partAddress.Add(City);
        if (!string.IsNullOrEmpty(Country)) partAddress.Add(Country);
        return string.Join(", ", partAddress);
    }

    private void UpdateGeometry()
    {
        if (_longitude.HasValue && _latitude.HasValue)
        {
            _geometry = new Point(_longitude.Value, _latitude.Value) { SRID = 4326 };
        }
        else
        {
            _geometry = null;
        }
    }
}
