using MyExpenses.Application.Dtos.Categories;
using CategoryTypeViewModel = MyExpenses.Presentation.ViewModels.Expenses.CategoryTypeViewModel;

namespace MyExpenses.Presentation.Mappings.Interfaces;

public interface IExpenseDtoViewModelMapper
{
    public CategoryTypeViewModel MapToViewModel(CategoryTypeDto src);

    public CategoryTypeDto MapToDto(CategoryTypeViewModel src);
}