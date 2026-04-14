using Domain.Models.Categories;
using Domain.Models.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.SharedUtils;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Mappings;

namespace MyExpenses.Sql.Repositories;

public class CategoryRepository(IDbContextFactory<DataBaseContext> dbContextFactory,
    ILogger<CategoryRepository> logger) : ICategoryRepository
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

    public async Task<Result> AddCategoryTypeAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default)
    {
        var categoryType = categoryTypeDomain.MapToEntity();

        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Adding category type with name {CategoryTypeName}", categoryType.Name);

        var json = categoryType.ToJson();
        logger.LogInformation("Category type json: {Json}", json);

        try
        {
            context.TCategoryTypes.Add(categoryType);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Category type with name {CategoryTypeName} was successfully added", categoryType.Name);
            return Result.Success("Category type was successfully added");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add category type with name {CategoryTypeName}", categoryType.Name);
            return Result.Failure(ErrorCode.DatabaseError, "Failed to add category type");
        }
    }
}