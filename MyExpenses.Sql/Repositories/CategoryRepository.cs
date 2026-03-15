using Domain.Models.Categories;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Mappings;

namespace MyExpenses.Sql.Repositories;

public class CategoryRepository(IDbContextFactory<DataBaseContext> dbContextFactory) : ICategoryRepository
{
    public async Task<IEnumerable<CategoryTypeDomain>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default)
    {
        await using var dataBaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dataBaseContext.TCategoryTypes
            .AsNoTracking()
            .Include(s => s.ColorFkNavigation)
            .ProjectToDomain()
            .ToListAsync(cancellationToken);
    }
}