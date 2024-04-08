using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_place")]
public partial class TPlace
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

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }

    [InverseProperty("PlaceFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();
}
