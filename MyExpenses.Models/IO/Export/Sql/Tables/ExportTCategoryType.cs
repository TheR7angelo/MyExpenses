using System.ComponentModel;
using CsvHelper.Configuration.Attributes;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTCategoryType
{
    [Name("id")]
    [DisplayName("id")]
    public int Id { get; set; }

    [Name("name")]
    [DisplayName("name")]
    public string? Name { get; set; }

    [Name("color_fk")]
    [DisplayName("color_fk")]
    public int? ColorFk { get; set; }

    [Name("date_added")]
    [DisplayName("date_added")]
    public DateTime? DateAdded { get; set; }
}