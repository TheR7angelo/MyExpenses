using System.ComponentModel;
using CsvHelper.Configuration.Attributes;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTHistory
{
    [Name("id")]
    [DisplayName("id")]
    public int Id { get; set; }
    
    [Name("account_fk")]
    [DisplayName("account_fk")]
    public int? AccountFk { get; set; }
    
    [Name("description")]
    [DisplayName("description")]
    public string? Description { get; set; }
    
    [Name("category_type_fk")]
    [DisplayName("category_type_fk")]
    public int? CategoryTypeFk { get; set; }
    
    [Name("mode_payment_fk")]
    [DisplayName("mode_payment_fk")]
    public int? ModePaymentFk { get; set; }
    
    [Name("value")]
    [DisplayName("value")]
    public double? Value { get; set; }
    
    [Name("date")]
    [DisplayName("date")]
    public DateTime? Date { get; set; }
    
    [Name("place_fk")]
    [DisplayName("place_fk")]
    public int? PlaceFk { get; set; }

    [Name("pointed")]
    [DisplayName("pointed")]
    public bool? Pointed { get; set; }

    [Name("bank_transfer_fk")]
    [DisplayName("bank_transfer_fk")]
    public int? BankTransferFk { get; set; }

    [Name("recursive_expense_fk")]
    [DisplayName("recursive_expense_fk")]
    public int? RecursiveExpenseFk { get; set; }

    [Name("date_added")]
    [DisplayName("date_added")]
    public DateTime? DateAdded { get; set; }

    [Name("date_pointed")]
    [DisplayName("date_pointed")]
    public DateTime? DatePointed { get; set; }
}