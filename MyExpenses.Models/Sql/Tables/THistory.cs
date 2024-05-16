using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_history")]
public partial class THistory : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("account_fk")]
    public int? AccountFk { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("category_type_fk")]
    public int? CategoryTypeFk { get; set; }

    [Column("mode_payment_fk")]
    public int? ModePaymentFk { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; }

    [Column("place_fk")]
    public int? PlaceFk { get; set; }

    [Column("pointed", TypeName = "BOOLEAN")]
    public bool? Pointed { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }

    [ForeignKey("AccountFk")]
    [InverseProperty("THistories")]
    public virtual TAccount? AccountFkNavigation { get; set; }

    [ForeignKey("CategoryTypeFk")]
    [InverseProperty("THistories")]
    public virtual TCategoryType? CategoryTypeFkNavigation { get; set; }

    [ForeignKey("ModePaymentFk")]
    [InverseProperty("THistories")]
    public virtual TModePayment? ModePaymentFkNavigation { get; set; }

    [ForeignKey("PlaceFk")]
    [InverseProperty("THistories")]
    public virtual TPlace? PlaceFkNavigation { get; set; }

    [InverseProperty("FromHistoryFkNavigation")]
    public virtual ICollection<TBankTransfer> TBankTransferFromHistoryFkNavigations { get; set; } = new List<TBankTransfer>();

    [InverseProperty("ToHistoryNavigation")]
    public virtual ICollection<TBankTransfer> TBankTransferToHistoryNavigations { get; set; } = new List<TBankTransfer>();
}
