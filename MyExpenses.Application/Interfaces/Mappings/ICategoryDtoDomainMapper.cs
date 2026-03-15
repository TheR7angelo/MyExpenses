using Domain.Models.Categories;
using MyExpenses.Application.Dtos.Categories;

namespace MyExpenses.Application.Interfaces.Mappings;

public interface ICategoryDtoDomainMapper
{
    public CategoryTypeDto MapToDto(CategoryTypeDomain src);
}