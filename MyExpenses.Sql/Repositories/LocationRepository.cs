using Domain.Models.Systems;
using Domain.Models.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Mappings;

namespace MyExpenses.Sql.Repositories;

public class LocationRepository(IDbContextFactory<DataBaseContext> dbContextFactory,
    ILogger<LocationRepository> logger) : ILocationRepository
{
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

    public async Task<Result<IEnumerable<PlaceDomain>>> GetAllPlaces(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            logger.LogInformation("Loading all places");

            var places = await context.TPlaces
                .AsNoTracking()
                .ProjectToDomain()
                .ToListAsync(cancellationToken);

            logger.LogInformation("Loaded {Count} places", places.Count);

            return Result<IEnumerable<PlaceDomain>>.Success(places);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load places");
            return Result<IEnumerable<PlaceDomain>>.Failure(ErrorCode.DatabaseError, "Failed to load places");
        }
    }
}