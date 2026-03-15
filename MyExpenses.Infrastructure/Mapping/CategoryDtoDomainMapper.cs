using Domain.Models.Categories;
using MyExpenses.Application.Dtos.Categories;
using MyExpenses.Application.Interfaces.Mappings;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Infrastructure.Mapping;

[Mapper]
public partial class CategoryDtoDomainMapper : ICategoryDtoDomainMapper
{
    public partial CategoryTypeDto MapToDto(CategoryTypeDomain src);
}