using System.ComponentModel;
using CsvHelper.Configuration.Attributes;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTSpatialRefSy
{
    [Name("srid")]
    [DisplayName("srid")]
    public int Srid { get; set; }

    [Name("auth_name")]
    [DisplayName("auth_name")]
    public string AuthName { get; set; }

    [Name("auth_srid")]
    [DisplayName("auth_srid")]
    public string AuthSrid { get; set; }

    [Name("srtext")]
    [DisplayName("srtext")]
    public string Srtext { get; set; }

    [Name("proj4text")]
    [DisplayName("proj4text")]
    public string Proj4text { get; set; }
}