using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using MyExpenses.Models.IO.Sig.Interfaces;
using NetTopologySuite.Geometries;

public class ExportTPlace : ISig
{
    [ColumnAttribute("id")]
    [DisplayName("id")]
    public int Id { get; set; }
    
    [ColumnAttribute("name")]
    [DisplayName("name")]
    public string? Name { get; set; }

    [ColumnAttribute("number")]
    [DisplayName("number")]
    public string? Number { get; set; }

    [ColumnAttribute("street")]
    [DisplayName("street")]
    public string? Street { get; set; }

    [ColumnAttribute("postal")]
    [DisplayName("postal")]
    public string? Postal { get; set; }

    [ColumnAttribute("city")]
    [DisplayName("city")]
    public string? City { get; set; }

    [ColumnAttribute("country")]
    [DisplayName("country")]
    public string? Country { get; set; }

    private double? _latitude;

    [ColumnAttribute("latitude")]
    [DisplayName("latitude")]
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
    [DisplayName("longitude")]
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
    [DisplayName("is_open")]
    public int? IsOpen { get; set; }

    [ColumnAttribute("can_be_deleted")]
    [DisplayName("can_be_deleted")]
    public int? CanBeDeleted { get; set; }

    [ColumnAttribute("date_added")]
    [DisplayName("date_added")]
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