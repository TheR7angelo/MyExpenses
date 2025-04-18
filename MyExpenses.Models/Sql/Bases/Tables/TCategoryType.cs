﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Tables;

[AddINotifyPropertyChangedInterface]
[Table("t_category_type")]
public partial class TCategoryType : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [MaxLength(55)]
    public string? Name { get; set; }

    [Column("color_fk")]
    public int? ColorFk { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; init; } = DateTime.Now;

    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    [ForeignKey("ColorFk")]
    [InverseProperty("TCategoryTypes")]
    public virtual TColor? ColorFkNavigation { get; set; }

    // Each ICollection property is initialized to prevent null references
    // and to ensure the collections are ready for use, even if no data is loaded from the database.
    // ReSharper disable HeapView.ObjectAllocation.Evident
    [InverseProperty("CategoryTypeFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();

    [InverseProperty("CategoryTypeFkNavigation")]
    public virtual ICollection<TRecursiveExpense> TRecursiveExpenses { get; set; } = new List<TRecursiveExpense>();
    // ReSharper restore HeapView.ObjectAllocation.Evident
    // ReSharper restore PropertyCanBeMadeInitOnly.Global
}
