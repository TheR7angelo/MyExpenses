using Domain.Models.Systems;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Mappings;

namespace MyExpenses.Sql.Repositories;

public class SystemRepository(IDbContextFactory<DataBaseContext> dbContextFactory,
    ILogger<SystemRepository> logger) : ISystemRepository
{
    public async Task<int> GetAllColorCount(CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Counting all colors available");
        var count = await context.TColors.CountAsync(cancellationToken);
        logger.LogInformation("Loaded {Count} colors", count);

        return count;
    }

    public async Task<ColorDomain> GetRandomColor(CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var colorCount = await GetAllColorCount(cancellationToken);
        var randomIndex = Random.Shared.Next(0, colorCount);

        logger.LogInformation("Getting random color with index {Index}", randomIndex);
        var randomColor = await context.TColors
            .AsNoTracking()
            .Skip(randomIndex)
            .ProjectToDomain()
            .FirstAsync(cancellationToken);

        logger.LogInformation("Loaded random color with name {Name}", randomColor.Name);
        return randomColor;
    }

    public async Task<IEnumerable<ColorDomain>> GetAllColors(CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all colors");

        var colors = await context.TColors
            .AsNoTracking()
            .ProjectToDomain()
            .ToListAsync(cancellationToken);
        logger.LogInformation("Loaded {Count} colors", colors.Count);

        return colors;
    }

    public async Task<PlaceDomain?> GetPlace(int placeId, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading place with id {PlaceId}", placeId);
        var place = await context.TPlaces
            .AsNoTracking()
            .ProjectToDomain()
            .FirstOrDefaultAsync(s => s.Id == placeId, cancellationToken);

        logger.LogInformation("Loaded place with id {PlaceId}, name {Name}", placeId, place?.Name);

        return place;
    }

    public async Task<bool> IsColorNameAvailableAsync(string name, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Checking availability of color name {Name}", name);

        var available = !await context.TColors
            .AnyAsync(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase), cancellationToken);

        logger.LogInformation("Color name {Name} is {Available}", name, available ? "available" : "taken");

        return available;
    }

    public async Task<bool> IsColorHexadecimalCodeAvailableAsync(string hexadecimalCode, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Checking availability of color hexadecimal code {HexadecimalCode}", hexadecimalCode);

        var available = !await context.TColors
            .AnyAsync(c => c.HexadecimalColorCode.Equals(hexadecimalCode, StringComparison.OrdinalIgnoreCase), cancellationToken);

        logger.LogInformation("Color hexadecimal code {HexadecimalCode} is {Available}", hexadecimalCode, available ? "available" : "taken");

        return available;
    }
}