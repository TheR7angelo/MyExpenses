using MyExpenses.Application.Dtos.Categories;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;

namespace MyExpenses.Infrastructure.Services;

public class CategoryService(ICategoryRepository categoryRepository, ICategoryDtoDomainMapper mapper) : ICategoryService
{
    public async Task<IEnumerable<CategoryTypeDto>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await categoryRepository.GetAllCategoryTypesAsync(cancellationToken);
        return categories.Select(mapper.MapToDto);
    }
}