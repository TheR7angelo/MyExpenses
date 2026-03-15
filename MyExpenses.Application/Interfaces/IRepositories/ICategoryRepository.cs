using Domain.Models.Categories;

namespace MyExpenses.Application.Interfaces.IRepositories;

public interface ICategoryRepository
{
    public Task<IEnumerable<CategoryTypeDomain>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default);
}