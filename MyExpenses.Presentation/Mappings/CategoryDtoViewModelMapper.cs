using MyExpenses.Application.Dtos.Categories;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.ViewModels.Categories;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Presentation.Mappings;

[Mapper]
public partial class CategoryDtoViewModelMapper : ICategoryDtoViewModelMapper
{
    public partial CategoryTypeViewModel MapToViewModel(CategoryTypeDto src);
}