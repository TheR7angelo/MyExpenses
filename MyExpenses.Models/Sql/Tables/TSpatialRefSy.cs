using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_spatial_ref_sys")]
public partial class TSpatialRefSy
{
    [Key]
    [Column("srid")]
    public int Srid { get; set; }

    [Column("auth_name")]
    public string AuthName { get; set; } = null!;

    [Column("auth_srid")]
    public string AuthSrid { get; set; } = null!;

    [Column("srtext")]
    public string Srtext { get; set; } = null!;

    [Column("proj4text")]
    public string Proj4text { get; set; } = null!;

    [InverseProperty("Sr")]
    public virtual ICollection<TGeometryColumn> TGeometryColumns { get; set; } = new List<TGeometryColumn>();
}
