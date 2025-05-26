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
    [MaxLength(155)]
    public string? Name { get; set; }

    [Column("number")]
    [MaxLength(20)]
    public string? Number { get; set; }

    [Column("street")]
    [MaxLength(155)]
    public string? Street { get; set; }

    [Column("postal")]
    [MaxLength(10)]
    public string? Postal { get; set; }

    [Column("city")]
    [MaxLength(100)]
    public string? City { get; set; }

    [Column("country")]
    [MaxLength(55)]
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
                Longitude = null;
                Latitude = null;
            }
            else
            {
                var point = (Point)_geometry;
                Longitude = point.X;
                Latitude = point.Y;
            }
        }
    }

    [Column("is_open", TypeName = "BOOLEAN")]
    public bool IsOpen { get; set; } = true;

    [Column("can_be_deleted", TypeName = "BOOLEAN")]
    public bool? CanBeDeleted { get; init; } = true;

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;

    // Each ICollection property is initialized to prevent null references
    // and to ensure the collections are ready for use, even if no data is loaded from the database.
    // ReSharper disable HeapView.ObjectAllocation.Evident
    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    [InverseProperty("PlaceFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();

    [InverseProperty("PlaceFkNavigation")]
    public virtual ICollection<TRecursiveExpense> TRecursiveExpenses { get; set; } = new List<TRecursiveExpense>();
    // ReSharper restore PropertyCanBeMadeInitOnly.Global
    // ReSharper restore HeapView.ObjectAllocation.Evident

    public override string ToString()
    {
        // This implementation was chosen based on performance benchmarks.
        // It uses a List<string> to collect non-empty components of the address,
        // which is both fast and memory-efficient for the given use case.
        // Alternative approaches, such as StringBuilder or LINQ, introduced
        // either higher memory allocation or slower execution times.
        // This method achieves the best balance between simplicity, performance,
        // and maintainability.
        // ReSharper disable once HeapView.ObjectAllocation.Evident
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
            // This implementation was chosen for its clarity and efficiency.
            // It ensures that the geometry is updated only when both longitude and
            // latitude have valid values, minimizing unnecessary object creation.
            // Setting _geometry to null when values are invalid ensures consistent state
            // management without introducing additional complexity.
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _geometry = new Point(_longitude.Value, _latitude.Value) { SRID = 4326 };
        }
        else
        {
            _geometry = null;
        }
    }
}
