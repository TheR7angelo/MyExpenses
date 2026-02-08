using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MyExpenses.Models.Sql.Bases.Tables;

[ObservableObject]
[Table("t_color")]
public partial class TColor : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set => SetProperty(ref field, value); }

    [Column("name")]
    [MaxLength(55)]
    public string? Name { get; set => SetProperty(ref field, value); }

    [Column("hexadecimal_color_code", TypeName = "TEXT(9)")]
    [MaxLength(9)]
    public string? HexadecimalColorCode { get; set => SetProperty(ref field, value); }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; init; } = DateTime.Now;

    // ICollection property is initialized to prevent null references
    // and to ensure the collection are ready for use, even if no data is loaded from the database.
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    [InverseProperty("ColorFkNavigation")]
    public virtual ICollection<TCategoryType> TCategoryTypes { get; set; } = new List<TCategoryType>();
    // ReSharper restore PropertyCanBeMadeInitOnly.Global
}
