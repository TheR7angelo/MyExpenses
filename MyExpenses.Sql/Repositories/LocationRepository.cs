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

    public async Task<Result<PlaceDomain>> CreatePlaceAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            logger.LogInformation("Adding place with name {PlaceName}", placeDomain.Name);

            var placeEntity = placeDomain.MapToEntity();
            context.TPlaces.Add(placeEntity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Place with name {PlaceName} was successfully added", placeDomain.Name);

            var placeDomainResult = placeEntity.MapToDomain();
            return Result<PlaceDomain>.Success(placeDomainResult, "Place was successfully added");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add place");
            return Result<PlaceDomain>.Failure(ErrorCode.DatabaseError, "Failed to add place");
        }
    }

    public async Task<Result<PlaceDomain>> UpdatePlaceAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            var place = placeDomain.MapToEntity();

            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            logger.LogInformation("Updating place with id {PlaceId}", place.Id);

            var entity = await context.TPlaces.FirstOrDefaultAsync(s => s.Id == place.Id, cancellationToken);
            if (entity is null)
            {
                logger.LogWarning("Place with id {PlaceId} not found", place.Id);
                return Result<PlaceDomain>.Failure(ErrorCode.NotFound, "Place not found");
            }

            place.Merge(entity);

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Place with id {PlaceId} was successfully updated", place.Id);
            return Result<PlaceDomain>.Success(placeDomain);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update place with id {PlaceId}", placeDomain.Id);
            return Result<PlaceDomain>.Failure(ErrorCode.DatabaseError, e.Message);
        }
    }
}