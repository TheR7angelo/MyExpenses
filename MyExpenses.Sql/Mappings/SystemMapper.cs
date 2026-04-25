using Domain.Models.Systems;
using MyExpenses.Models.Sql.Bases.Tables;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Sql.Mappings;

[Mapper]
public static partial class SystemMapper
{
    public static partial IQueryable<ColorDomain> ProjectToDomain(this IQueryable<TColor> src);

    [MapperIgnoreSource(nameof(TColor.TCategoryTypes))]
    public static partial ColorDomain MapToDomain(TColor src);

    [MapperIgnoreTarget(nameof(TColor.TCategoryTypes))]
    public static partial TColor MapToEntity(this ColorDomain src);
}