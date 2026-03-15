using Domain.Models.Systems;
using MyExpenses.Models.Sql.Bases.Tables;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Sql.Mappings;

[Mapper]
public partial class SystemMapper
{
    [MapperIgnoreSource(nameof(TColor.TCategoryTypes))]
    public partial ColorDomain MapToDomain(TColor src);
}