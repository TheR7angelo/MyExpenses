﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyExpenses.Models.Sql.Bases.Enums;

namespace MyExpenses.Models.Sql.Bases.Tables;

[Table("t_recursive_frequency")]
public partial class TRecursiveFrequency
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [NotMapped]
    public ERecursiveFrequency ERecursiveFrequency
        => (ERecursiveFrequency)Id;

    [Column("frequency")]
    public string? Frequency { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [InverseProperty("FrequencyFkNavigation")]
    public virtual ICollection<TRecursiveExpense> TRecursiveExpenses { get; set; } = new List<TRecursiveExpense>();
}
