using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Bases.Tables;

[Table("t_spatial_ref_sys")]
public class TSpatialRefSy
{
    [Key]
    [Column("srid")]
    public int Srid { get; set; }

    [Column("auth_name")]
    [MaxLength(50)]
    public string AuthName { get; set; } = null!;

    [Column("auth_srid")]
    [MaxLength(10)]
    public string AuthSrid { get; set; } = null!;

    [Column("srtext")]
    [MaxLength(2000)]
    public string Srtext { get; set; } = null!;

    [Column("proj4text")]
    [MaxLength(255)]
    public string Proj4text { get; set; } = null!;
}
