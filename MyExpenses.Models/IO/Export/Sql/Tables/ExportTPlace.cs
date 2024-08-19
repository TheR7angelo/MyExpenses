using System.ComponentModel.DataAnnotations.Schema;
using MyExpenses.Models.IO.Sig.Interfaces;
using NetTopologySuite.Geometries;

public class ExportTPlace : ISig
{
    [Column("id")]
    public int Id { get; set; }
    
    [ColumnAttribute("name")]
    public string? Name { get; set; }

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

    [ColumnAttribute("is_open")]
    public bool? IsOpen { get; set; } = true;

    [ColumnAttribute("can_be_deleted")]
    public bool? CanBeDeleted { get; set; } = true;

    [ColumnAttribute("date_added")]
    public DateTime? DateAdded { get; set; }

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