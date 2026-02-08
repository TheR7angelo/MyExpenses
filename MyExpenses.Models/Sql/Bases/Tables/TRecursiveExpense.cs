using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using MyExpenses.Models.Attributs;
using MyExpenses.Models.Sql.Bases.Enums;

namespace MyExpenses.Models.Sql.Bases.Tables;

[ObservableObject]
[Table("t_recursive_expense")]
public partial class TRecursiveExpense : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("account_fk")]
    public int? AccountFk { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set => SetProperty(ref field, value); }

    [Column("note")]
    [MaxLength(255)]
    public string? Note { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("category_type_fk")]
    public int? CategoryTypeFk { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("mode_payment_fk")]
    public int? ModePaymentFk { get; set => SetProperty(ref field, value); }

    [NotMapped]
    public EModePayment EModePaymentFk => TModePayment.GetModePayment(ModePaymentFk);

    [Required]
    [Column("value")]
    public double? Value { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("place_fk")]
    public int? PlaceFk { get; set => SetProperty(ref field, value); }

    [IgnoreReset]
    [Required]
    [Column("start_date", TypeName = "DATE")]
    public DateOnly StartDate { get; set => SetProperty(ref field, value); }

    [Column("recursive_total")]
    public int? RecursiveTotal { get; set => SetProperty(ref field, value); }

    [Column("recursive_count")]
    public int RecursiveCount { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("frequency_fk")]
    public int FrequencyFk { get; set => SetProperty(ref field, value); }

    [NotMapped]
    public ERecursiveFrequency ERecursiveFrequency
    {
        get => (ERecursiveFrequency)FrequencyFk;
        init => FrequencyFk = (int)value;
    }

    [Column("next_due_date", TypeName = "DATE")]
    public DateOnly NextDueDate { get; set => SetProperty(ref field, value); }

    [Column("is_active", TypeName = "BOOLEAN")]
    public bool IsActive { get; set => SetProperty(ref field, value); }

    [Column("force_deactivate", TypeName = "BOOLEAN")]
    public bool ForceDeactivate { get; set => SetProperty(ref field, value); }

    [IgnoreReset]
    [Column("date_added", TypeName = "DATE")]
    public DateTime? DateAdded { get; private set => SetProperty(ref field, value); }

    [Column("last_updated", TypeName = "DATE")]
    public DateTime? LastUpdated { get; set => SetProperty(ref field, value); }

    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    [ForeignKey("AccountFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TAccount? AccountFkNavigation { get; set => SetProperty(ref field, value); }

    [ForeignKey("CategoryTypeFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TCategoryType? CategoryTypeFkNavigation { get; set => SetProperty(ref field, value); }

    [ForeignKey("FrequencyFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TRecursiveFrequency FrequencyFkNavigation { get; set => SetProperty(ref field, value); } = null!;

    [ForeignKey("ModePaymentFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TModePayment? ModePaymentFkNavigation { get; set => SetProperty(ref field, value); }

    [ForeignKey("PlaceFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TPlace? PlaceFkNavigation { get; set => SetProperty(ref field, value); }

    [InverseProperty("RecursiveExpenseFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; }
    // ReSharper restore PropertyCanBeMadeInitOnly.Global
}
