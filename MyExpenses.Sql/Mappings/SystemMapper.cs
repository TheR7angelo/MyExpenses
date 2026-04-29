using Domain.Models.Systems;
using MyExpenses.Models.Sql.Bases.Tables;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Sql.Mappings;

[Mapper]
public static partial class SystemMapper
{
    public static partial IQueryable<ColorDomain> ProjectToDomain(this IQueryable<TColor> src);

    public static partial IQueryable<PlaceDomain> ProjectToDomain(this IQueryable<TPlace> src);
    public static partial IQueryable<RecursiveFrequencyDomain> ProjectToDomain(this IQueryable<TRecursiveFrequency> src);

    [MapperIgnoreSource(nameof(TColor.TCategoryTypes))]
    public static partial ColorDomain MapToDomain(this TColor src);

    [MapperIgnoreTarget(nameof(TColor.TCategoryTypes))]
    public static partial TColor MapToEntity(this ColorDomain src);

    [MapperIgnoreSource(nameof(TPlace.THistories))]
    [MapperIgnoreSource(nameof(TPlace.TRecursiveExpenses))]
    [MapperIgnoreSource(nameof(TPlace.Geometry))]
    public static partial PlaceDomain MapToDomain(this TPlace src);

    [MapperIgnoreTarget(nameof(TPlace.THistories))]
    [MapperIgnoreTarget(nameof(TPlace.TRecursiveExpenses))]
    [MapperIgnoreTarget(nameof(TPlace.Geometry))]
    public static partial TPlace MapToEntity(this PlaceDomain src);

    [MapperIgnoreSource(nameof(TRecursiveFrequency.TRecursiveExpenses))]
    [MapperIgnoreSource(nameof(TRecursiveFrequency.ERecursiveFrequency))]
    public static partial RecursiveFrequencyDomain MapToDomain(this TRecursiveFrequency src);

    [MapperIgnoreTarget(nameof(TRecursiveFrequency.TRecursiveExpenses))]
    [MapperIgnoreTarget(nameof(TRecursiveFrequency.ERecursiveFrequency))]
    public static partial TRecursiveFrequency MapToEntity(this RecursiveFrequencyDomain src);
}