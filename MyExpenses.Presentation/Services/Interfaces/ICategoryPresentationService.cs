using MyExpenses.Presentation.ViewModels.Categories;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface ICategoryPresentationService
{
    public Task<IEnumerable<CategoryTypeViewModel>> GetAllCategoryTypeViewModelAsync(CancellationToken cancellationToken = default);
}