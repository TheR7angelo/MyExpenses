using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyExpenses.Models.Sql.Bases.Enums;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Tables;

[AddINotifyPropertyChangedInterface]
[Table("t_recursive_expense")]
public partial class TRecursiveExpense : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("account_fk")]
    public int? AccountFk { get; set; }

    [Required]
    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set; }

    [Column("note")]
    [MaxLength(255)]
    public string? Note { get; set; }

    [Required]
    [Column("category_type_fk")]
    public int? CategoryTypeFk { get; set; }

    [Required]
    [Column("mode_payment_fk")]
    public int? ModePaymentFk { get; set; }

    [Required]
    [Column("value")]
    public double? Value { get; set; }

    [Required]
    [Column("place_fk")]
    public int? PlaceFk { get; set; }

    [Required]
    [Column("start_date", TypeName = "DATE")]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [Column("recursive_total")]
    public int? RecursiveTotal { get; set; }

    [Column("recursive_count")]
    public int RecursiveCount { get; set; }

    [Required]
    [Column("frequency_fk")]
    public int FrequencyFk { get; set; }

    [NotMapped]
    public ERecursiveFrequency ERecursiveFrequency
    {
        get => (ERecursiveFrequency)FrequencyFk;
        set => FrequencyFk = (int)value;
    }

    [Column("next_due_date", TypeName = "DATE")]
    public DateOnly NextDueDate { get; set; }

    [Column("is_active", TypeName = "BOOLEAN")]
    public bool IsActive { get; set; }

    [Column("force_deactivate", TypeName = "BOOLEAN")]
    public bool ForceDeactivate { get; set; }

    [Column("date_added", TypeName = "DATE")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;

    [Column("last_updated", TypeName = "DATE")]
    public DateTime? LastUpdated { get; set; }

    [ForeignKey("AccountFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TAccount? AccountFkNavigation { get; set; }

    [ForeignKey("CategoryTypeFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TCategoryType? CategoryTypeFkNavigation { get; set; }

    [ForeignKey("FrequencyFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TRecursiveFrequency FrequencyFkNavigation { get; set; } = null!;

    [ForeignKey("ModePaymentFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TModePayment? ModePaymentFkNavigation { get; set; }

    [ForeignKey("PlaceFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TPlace? PlaceFkNavigation { get; set; }

    [InverseProperty("RecursiveExpenseFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();
}
