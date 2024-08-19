using System.ComponentModel;
using CsvHelper.Configuration.Attributes;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTGeometryColumn
{
    [Name("id")]
    [DisplayName("id")]
    public int Id { get; set; }

    [Name("f_table_name")]
    [DisplayName("f_table_name")]
    public string FTableName { get; set; }

    [Name("f_geometry_column")]
    [DisplayName("f_geometry_column")]
    public string FGeometryName { get; set; }

    [Name("type")]
    [DisplayName("type")]
    public string Type { get; set; }

    [Name("coord_dimension")]
    [DisplayName("coord_dimension")]
    public int CoordDimension { get; set; }

    [Name("srid")]
    [DisplayName("srid")]
    public int Srid { get; set; }
}