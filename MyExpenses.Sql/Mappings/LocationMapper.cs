using Domain.Models.Systems;
using MyExpenses.Models.Sql.Bases.Tables;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Sql.Mappings;

[Mapper]
public static partial class LocationMapper
{
    public static partial IQueryable<PlaceDomain> ProjectToDomain(this IQueryable<TPlace> src);

    [MapperIgnoreSource(nameof(TPlace.THistories))]
    [MapperIgnoreSource(nameof(TPlace.TRecursiveExpenses))]
    [MapperIgnoreSource(nameof(TPlace.Geometry))]
    public static partial PlaceDomain MapToDomain(this TPlace src);

    [MapperIgnoreTarget(nameof(TPlace.THistories))]
    [MapperIgnoreTarget(nameof(TPlace.TRecursiveExpenses))]
    [MapperIgnoreTarget(nameof(TPlace.Geometry))]
    public static partial TPlace MapToEntity(this PlaceDomain src);
}