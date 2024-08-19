using System.ComponentModel;
using CsvHelper.Configuration.Attributes;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTModePayment
{
    [Name("id")]
    [DisplayName("id")]
    public int Id { get; set; }

    [Name("name")]
    [DisplayName("name")]
    public string? Name { get; set; }

    [Name("can_be_deleted")]
    [DisplayName("can_be_deleted")]
    public bool? CanBeDeleted { get; set; }

    [Name("date_added")]
    [DisplayName("date_added")]
    public DateTime? DateAdded { get; set; }
}