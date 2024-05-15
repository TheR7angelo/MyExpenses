﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Tables;

[AddINotifyPropertyChangedInterface]
[Table("t_currency")]
public partial class TCurrency : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("symbol")]
    public string? Symbol { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;

    [InverseProperty("CurrencyFkNavigation")]
    public virtual ICollection<TAccount> TAccounts { get; set; } = new List<TAccount>();
}
