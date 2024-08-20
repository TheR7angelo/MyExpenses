using System.ComponentModel;
using CsvHelper.Configuration.Attributes;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTCurrency
{
    [Name("id")]
    [DisplayName("id")]
    public int Id { get; set; }

    [Name("symbol")]
    [DisplayName("symbol")]
    public string? Symbol { get; set; }

    [Name("date_added")]
    [DisplayName("date_added")]
    public DateTime? DateAdded { get; set; }
}