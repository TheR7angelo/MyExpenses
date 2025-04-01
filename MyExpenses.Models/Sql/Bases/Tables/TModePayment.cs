using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyExpenses.Models.Sql.Bases.Enums;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Tables;

[AddINotifyPropertyChangedInterface]
[Table("t_mode_payment")]
public partial class TModePayment : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [MaxLength(55)]
    public string? Name { get; set; }

    [Column("can_be_deleted", TypeName = "BOOLEAN")]
    public bool? CanBeDeleted { get; init; } = true;

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; init; } = DateTime.Now;

    [NotMapped]
    public EModePayment EModePayment => GetModePayment(Id);

    // Each ICollection property is initialized to prevent null references
    // and to ensure the collections are ready for use, even if no data is loaded from the database.
    // ReSharper disable HeapView.ObjectAllocation.Evident
    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    [InverseProperty("ModePaymentFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();

    [InverseProperty("ModePaymentFkNavigation")]
    public virtual ICollection<TRecursiveExpense> TRecursiveExpenses { get; set; } = new List<TRecursiveExpense>();
    // ReSharper restore PropertyCanBeMadeInitOnly.Global
    // ReSharper restore HeapView.ObjectAllocation.Evident

    /// <summary>
    /// Retrieves the mode of payment corresponding to the provided identifier.
    /// </summary>
    /// <param name="id">The identifier of the mode of payment.</param>
    /// <returns>The <see cref="EModePayment"/> value corresponding to the given identifier. Returns <see cref="EModePayment.Another"/> if the identifier doesn't match any predefined mode.</returns>
    public static EModePayment GetModePayment(int? id)
    {
        return id switch
        {
            1 => EModePayment.BankCard,
            2 => EModePayment.BankTransfer,
            3 => EModePayment.BankDirectDebit,
            4 => EModePayment.BankCheck,
            _ => EModePayment.Another
        };
    }
}
