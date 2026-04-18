using Domain.Models.Validation;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Services;

public class ExpensePresentationService(IExpenseService categoryService, IExpenseDtoViewModelMapper mapper) : IExpensePresentationService
{
    public async Task<IEnumerable<CategoryTypeViewModel>> GetAllCategoryTypeViewModelAsync(CancellationToken cancellationToken = default)
    {
        var categoryTypes = await categoryService.GetAllCategoryTypesAsync(cancellationToken);
        return categoryTypes.Select(mapper.MapToViewModel);
    }

    public Task<Result> AddCategoryType(CategoryTypeViewModel newCategoryType, CancellationToken cancellationToken = default)
    {
        var categoryTypeDto = mapper.MapToDto(newCategoryType);
        return categoryService.AddCategoryTypeAsync(categoryTypeDto, cancellationToken);
    }
}