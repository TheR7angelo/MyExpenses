using Domain.Models.Systems;
using MyExpenses.Models.Sql.Bases.Tables;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Sql.Mappings;

[Mapper]
public static partial class SystemMapper
{
    public static partial IQueryable<ColorDomain> ProjectToDomain(this IQueryable<TColor> src);

    public static partial IQueryable<RecursiveFrequencyDomain> ProjectToDomain(this IQueryable<TRecursiveFrequency> src);

    [MapperIgnoreSource(nameof(TColor.TCategoryTypes))]
    public static partial ColorDomain MapToDomain(this TColor src);

    [MapperIgnoreTarget(nameof(TColor.TCategoryTypes))]
    public static partial TColor MapToEntity(this ColorDomain src);

    public static partial void Merge(this TColor src, TColor dst);

    [MapperIgnoreSource(nameof(TRecursiveFrequency.TRecursiveExpenses))]
    [MapperIgnoreSource(nameof(TRecursiveFrequency.ERecursiveFrequency))]
    public static partial RecursiveFrequencyDomain MapToDomain(this TRecursiveFrequency src);

    [MapperIgnoreTarget(nameof(TRecursiveFrequency.TRecursiveExpenses))]
    [MapperIgnoreTarget(nameof(TRecursiveFrequency.ERecursiveFrequency))]
    public static partial TRecursiveFrequency MapToEntity(this RecursiveFrequencyDomain src);
}