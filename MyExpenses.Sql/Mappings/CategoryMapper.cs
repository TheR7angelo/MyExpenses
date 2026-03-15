using Domain.Models.Categories;
using Domain.Models.Systems;
using MyExpenses.Models.Sql.Bases.Tables;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Sql.Mappings;

[Mapper]
public static partial class CategoryMapper
{
    public static partial IQueryable<CategoryTypeDomain> ProjectToDomain(this IQueryable<TCategoryType> src);

    [MapperIgnoreSource(nameof(TCategoryType.THistories))]
    [MapperIgnoreSource(nameof(TCategoryType.TRecursiveExpenses))]
    [MapperIgnoreSource(nameof(TCategoryType.ColorFk))]
    [MapProperty(nameof(TCategoryType.ColorFkNavigation), nameof(CategoryTypeDomain.Color))]
    public static partial CategoryTypeDomain MapToDomain(TCategoryType src);

    private static ColorDomain MapColor(TColor src) => new SystemMapper().MapToDomain(src);
}