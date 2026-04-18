using Domain.Models.Categories;
using MyExpenses.Application.Dtos.Categories;

namespace MyExpenses.Application.Interfaces.Mappings;

public interface IExpenseDtoDomainMapper
{
    public CategoryTypeDto MapToDto(CategoryTypeDomain src);
}