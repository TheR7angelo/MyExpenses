﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Views;

[Keyless]
public partial class VRecursiveExpense
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("account_fk")]
    public int? AccountFk { get; set; }

    [Column("account")]
    public string? Account { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("category_type_fk")]
    public int? CategoryTypeFk { get; set; }

    [Column("category")]
    public string? Category { get; set; }

    [Column("mode_payment_fk")]
    public int? ModePaymentFk { get; set; }

    [Column("mode_payment")]
    public string? ModePayment { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("place_fk")]
    public int? PlaceFk { get; set; }

    [Column("place")]
    public string? Place { get; set; }

    [Column("start_date", TypeName = "DATE")]
    public DateTime? StartDate { get; set; }

    [Column("recursive_total")]
    public int? RecursiveTotal { get; set; }

    [Column("recursive_count")]
    public int? RecursiveCount { get; set; }

    [Column("frequency_fk")]
    public int? FrequencyFk { get; set; }

    [Column("frequency")]
    public string? Frequency { get; set; }

    [Column("next_due_date", TypeName = "DATE")]
    public DateTime? NextDueDate { get; set; }

    [Column("is_active", TypeName = "BOOLEAN")]
    public bool? IsActive { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }
}
