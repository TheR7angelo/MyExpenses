using System.ComponentModel;
using CsvHelper.Configuration.Attributes;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTBankTransfer
{
    [Name("id")]
    [DisplayName("id")]
    public int Id { get; set; }

    [Name("value")]
    [DisplayName("value")]
    public double? Value { get; set; }

    [Name("from_account_fk")]
    [DisplayName("from_account_fk")]
    public int? FromAccountFk { get; set; }

    [Name("to_account_fk")]
    [DisplayName("to_account_fk")]
    public int? ToAccountFk { get; set; }

    [Name("main_reason")]
    [DisplayName("main_reason")]
    public string? MainReason { get; set; }

    [Name("additional_reason")]
    [DisplayName("additional_reason")]
    public string? AdditionalReason { get; set; }

    [Name("date")]
    [DisplayName("date")]
    public DateTime? Date { get; set; }

    [Name("date_added")]
    [DisplayName("date_added")]
    public DateTime? DateAdded { get; set; }
}