using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_geometry_columns")]
public partial class TGeometryColumn
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("f_table_name")]
    public string FTableName { get; set; } = null!;

    [Column("f_geometry_column")]
    public string FGeometryColumn { get; set; } = null!;

    [Column("type")]
    public string Type { get; set; } = null!;

    [Column("coord_dimension")]
    public int CoordDimension { get; set; }

    [Column("srid")]
    public int Srid { get; set; }

    [ForeignKey("Srid")]
    [InverseProperty("TGeometryColumns")]
    public virtual TSpatialRefSy Sr { get; set; } = null!;
}
