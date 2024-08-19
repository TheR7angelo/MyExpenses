using System.ComponentModel;
using CsvHelper.Configuration.Attributes;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTColor
{
    [Name("id")]
    [DisplayName("id")]
    public int Id { get; set; }

    [Name("name")]
    [DisplayName("name")]
    public string? Name { get; set; }

    [Name("hexadecimal_color_code")]
    [DisplayName("hexadecimal_color_code")]
    public string? HexadecimalColorCode { get; set; }

    [Name("date_added")]
    [DisplayName("date_added")]
    public DateTime? DateAdded { get; set; }
}