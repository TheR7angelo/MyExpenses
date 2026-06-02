using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;

namespace MyExpenses.Infrastructure.Services;

public class LocationService(ILocationRepository locationRepository,
    ILocationDtoDomainMapper locationDtoDomainMapper) : ILocationService
{
    public async Task<PlaceDto?> GetPlace(int placeId, CancellationToken cancellationToken)
    {
        var place = await locationRepository.GetPlace(placeId, cancellationToken);
        if (place is null) return null;

        var placeDto = locationDtoDomainMapper.MapToDto(place);
        return placeDto;
    }

    public async Task<Result<IEnumerable<PlaceDto>>> GetAllPlaces(CancellationToken cancellationToken = default)
    {
        var result = await locationRepository.GetAllPlaces(cancellationToken);
        if (!result.IsSuccess) return Result<IEnumerable<PlaceDto>>.Failure(result.ErrorCode, result.InternalMessage!);

        var placeDtos = result.Value!.Select(locationDtoDomainMapper.MapToDto);
        return Result<IEnumerable<PlaceDto>>.Success(placeDtos);
    }

    public async Task<Result<PlaceDto>> CreatePlaceAsync(PlaceDto placeDto, CancellationToken cancellationToken = default)
    {
        var placeDomain = locationDtoDomainMapper.MapToDomain(placeDto);
        var result = await locationRepository.CreatePlaceAsync(placeDomain, cancellationToken);
        return result.IsSuccess
            ? Result<PlaceDto>.Success(locationDtoDomainMapper.MapToDto(result.Value!))
            : Result<PlaceDto>.Failure(result.ErrorCode, result.InternalMessage!);
    }
}