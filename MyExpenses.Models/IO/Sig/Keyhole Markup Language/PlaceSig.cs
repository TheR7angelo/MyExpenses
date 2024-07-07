using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyExpenses.Models.IO.Sig.Interfaces;
using NetTopologySuite.Geometries;

namespace MyExpenses.Models.IO.Sig.Keyhole_Markup_Language;

[Table("t_place")]
public class PlaceSig : ISig
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

    [Column("latitude")]
    public double? Latitude { get; set; }

    [Column("longitude")]
    public double? Longitude { get; set; }

    public Geometry? Geometry
    {
        get =>
            new Point(Latitude.GetValueOrDefault(), Longitude.GetValueOrDefault())
            {
                SRID = 4326
            };
        set
        {
            if (value is Point point)
            {
                Latitude = point.X;
                Longitude = point.Y;
            }
            else
            {
                Latitude = null;
                Longitude = null;
            }
        }
    }

    [Column("is_open")]
    public bool? IsOpen { get; set; }

    [Column("can_be_deleted")]
    public bool? CanBeDeleted { get; set; }

    [Column("date_added")]
    public DateTime? DateAdded { get; set; }
}