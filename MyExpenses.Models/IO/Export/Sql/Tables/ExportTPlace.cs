using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;
using MyExpenses.Models.IO.Sig.Interfaces;
using NetTopologySuite.Geometries;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTPlace : ISig
{
    [Name("id")]
    [DisplayName("id")]
    public int Id { get; set; }

    [Name("name")]
    [DisplayName("name")]
    public string? Name { get; set; }

    [Name("number")]
    [DisplayName("number")]
    public string? Number { get; set; }

    [Name("street")]
    [DisplayName("street")]
    public string? Street { get; set; }

    [Name("postal")]
    [DisplayName("postal")]
    public string? Postal { get; set; }

    [Name("city")]
    [DisplayName("city")]
    public string? City { get; set; }

    [Name("country")]
    [DisplayName("country")]
    public string? Country { get; set; }

    [NotMapped]
    private double? _latitude;

    [Name("latitude")]
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

    [Ignore]
    private double? _longitude;

    [Name("longitude")]
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

    [Ignore]
    private Geometry? _geometry;

    [Name("geometry")]
    [DisplayName("geometry")]
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

    [Name("is_open")]
    [DisplayName("is_open")]
    public bool? IsOpen { get; set; } = true;

    [Name("can_be_deleted")]
    [DisplayName("can_be_deleted")]
    public bool? CanBeDeleted { get; set; } = true;

    [Name("date_added")]
    [DisplayName("date_added")]
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