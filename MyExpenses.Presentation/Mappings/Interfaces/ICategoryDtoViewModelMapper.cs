using MyExpenses.Application.Dtos.Categories;
using MyExpenses.Presentation.ViewModels.Categories;

namespace MyExpenses.Presentation.Mappings.Interfaces;

public interface ICategoryDtoViewModelMapper
{
    public CategoryTypeViewModel MapToViewModel(CategoryTypeDto src);
}