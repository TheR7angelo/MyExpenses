using System.ComponentModel.DataAnnotations.Schema;
using MyExpenses.Models.IO.Sig.Interfaces;
using NetTopologySuite.Geometries;

public class ExportTPlace : ISig
{
    [Column("id")]
    public int Id { get; set; }
    
    [ColumnAttribute("ColumnAttribute")]
    public string? ColumnAttribute { get; set; }

    [ColumnAttribute("number")]
    public string? Number { get; set; }

    [ColumnAttribute("street")]
    public string? Street { get; set; }

    [ColumnAttribute("postal")]
    public string? Postal { get; set; }

    [ColumnAttribute("city")]
    public string? City { get; set; }

    [ColumnAttribute("country")]
    public string? Country { get; set; }

    private double? _latitude;

    [ColumnAttribute("latitude")]
    public double? Latitude
    {
        get => _latitude;
        set
        {
            _latitude = value;
            UpdateGeometry();
        }
    }

    private double? _longitude;

    [ColumnAttribute("longitude")]
    public double? Longitude
    {
        get => _longitude;
        set
        {
            _longitude = value;
            UpdateGeometry();
        }

    }

    private Geometry? _geometry;

    [ColumnAttribute("geometry")]
    public Geometry? Geometry
    {
        get => _geometry;
        set
        {
            _geometry = value;
            if (_geometry is null)
            {
                _latitude = null;
                _longitude = null;
            }
            else
            {
                var point = (Point)_geometry;
                _latitude = point.X;
                _longitude = point.Y;
            }
        }
    }

    [ColumnAttribute("is_open")]
    public bool? IsOpen { get; set; } = true;

    [ColumnAttribute("can_be_deleted")]
    public bool? CanBeDeleted { get; set; } = true;

    [ColumnAttribute("date_added")]
    public DateTime? DateAdded { get; set; }

    private void UpdateGeometry()
    {
        if (_latitude.HasValue && _longitude.HasValue)
        {
            _geometry = new Point(_latitude.Value, _longitude.Value) { SRID = 4326 };
        }
        else
        {
            _geometry = null;
        }
    }
}