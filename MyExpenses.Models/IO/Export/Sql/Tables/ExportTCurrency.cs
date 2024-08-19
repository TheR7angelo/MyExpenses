using CsvHelper.Configuration.Attributes;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTCurrency
{
    [Name("id")]
    public int Id { get; set; }

    [Name("symbol")]
    public string? Symbol { get; set; }

    [Name("date_added")]
    public DateTime? DateAdded { get; set; }
}