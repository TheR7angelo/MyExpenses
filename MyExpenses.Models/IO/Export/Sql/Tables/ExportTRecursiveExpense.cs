using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;
using MyExpenses.Models.Sql.Bases.Enums;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTRecursiveExpense
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

    [Name("note")]
    [DisplayName("note")]
    public string? Note { get; set; }
    
    [Name("category_type_fk")]
    [DisplayName("category_type_fk")]
    public int? CategoryTypeFk { get; set; }
    
    [Name("mode_payment_fk")]
    [DisplayName("mode_payment_fk")]
    public int? ModePaymentFk { get; set; }
    
    [Name("value")]
    [DisplayName("value")]
    public double? Value { get; set; }
    
    [Name("place_fk")]
    [DisplayName("place_fk")]
    public int? PlaceFk { get; set; }
    
    [Name("start_date")]
    [DisplayName("start_date")]
    public DateOnly StartDate { get; set; }

    [Name("recursive_total")]
    [DisplayName("recursive_total")]
    public int? RecursiveTotal { get; set; }

    [Name("recursive_count")]
    [DisplayName("recursive_count")]
    public int RecursiveCount { get; set; }
    
    [Name("frequency_fk")]
    [DisplayName("frequency_fk")]
    public int FrequencyFk { get; set; }

    [Ignore]
    public ERecursiveFrequency ERecursiveFrequency
    {
        get => (ERecursiveFrequency)FrequencyFk;
        set => FrequencyFk = (int)value;
    }

    [Name("next_due_date")]
    [DisplayName("next_due_date")]
    public DateOnly NextDueDate { get; set; }

    [Name("is_active")]
    [DisplayName("is_active")]
    public bool IsActive { get; set; }

    [Name("force_deactivate")]
    [DisplayName("force_deactivate")]
    public bool ForceDeactivate { get; set; }

    [Name("date_added")]
    [DisplayName("date_added")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;

    [Name("last_updated")]
    [DisplayName("last_updated")]
    public DateTime? LastUpdated { get; set; }
}