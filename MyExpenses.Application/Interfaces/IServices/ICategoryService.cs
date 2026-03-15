using MyExpenses.Application.Dtos.Categories;

namespace MyExpenses.Application.Interfaces.IServices;

public interface ICategoryService
{
    public Task<IEnumerable<CategoryTypeDto>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default);
}