using Domain.Models.Systems;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Mappings;

namespace MyExpenses.Sql.Repositories;

public class SystemRepository(IDbContextFactory<DataBaseContext> dbContextFactory) : ISystemRepository
{
    public async Task<int> GetAllColorCount(CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.TColors.CountAsync(cancellationToken);
    }

    public async Task<ColorDomain> GetRandomColor(CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var colorCount = await GetAllColorCount(cancellationToken);
        var randomIndex = Random.Shared.Next(0, colorCount);

        var randomColor = await context.TColors
            .AsNoTracking()
            .Skip(randomIndex)
            .ProjectToDomain()
            .FirstAsync(cancellationToken);

        return randomColor;
    }
}