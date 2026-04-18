using Domain.Models.Categories;
using MyExpenses.Application.Dtos.Categories;

namespace MyExpenses.Application.Interfaces.Mappings;

public interface IExpenseDtoDomainMapper
{
    /// <summary>
    /// Maps the CategoryTypeDomain object to a CategoryTypeDto object.
    /// </summary>
    /// <param name="src">The source CategoryTypeDomain object to map.</param>
    /// <returns>A CategoryTypeDto object that is mapped from the source object.</returns>
    public CategoryTypeDto MapToDto(CategoryTypeDomain src);

    /// <summary>
    /// Maps the CategoryTypeDto object to a CategoryTypeDomain object.
    /// </summary>
    /// <param name="src">The source CategoryTypeDto object to map.</param>
    /// <returns>A CategoryTypeDomain object that is mapped from the source object.</returns>
    public CategoryTypeDomain MapToDomain(CategoryTypeDto src);
}