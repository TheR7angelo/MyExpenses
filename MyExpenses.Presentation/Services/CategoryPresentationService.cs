using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Categories;

namespace MyExpenses.Presentation.Services;

public class CategoryPresentationService(ICategoryService categoryService, ICategoryDtoViewModelMapper mapper) : ICategoryPresentationService
{
    public async Task<IEnumerable<CategoryTypeViewModel>> GetAllCategoryTypeViewModelAsync(CancellationToken cancellationToken = default)
    {
        var categoryTypes = await categoryService.GetAllCategoryTypesAsync(cancellationToken);
        return categoryTypes.Select(mapper.MapToViewModel);
    }
}