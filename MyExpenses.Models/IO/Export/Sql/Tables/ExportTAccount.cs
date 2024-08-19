using System.ComponentModel;
using CsvHelper.Configuration.Attributes;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTAccount
{
    [Name("id")]
    [DisplayName("id")]
    public int Id { get; set; }

    [Name("name")]
    [DisplayName("name")]
    public string? Name { get; set; }

    [Name("account_type_fk")]
    [DisplayName("account_type_fk")]
    public int? AccountTypeFk { get; set; }

    [Name("currency_fk")]
    [DisplayName("currency_fk")]
    public int? CurrencyFk { get; set; }

    [Name("active")]
    [DisplayName("active")]
    public bool? Active { get; set; }

    [Name("date_added")]
    [DisplayName("date_added")]
    public DateTime? DateAdded { get; set; }
}