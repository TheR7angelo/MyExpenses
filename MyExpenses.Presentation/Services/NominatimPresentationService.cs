using Domain.Models.Validation;
using Mapsui;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Locations;

namespace MyExpenses.Presentation.Services;

public class NominatimPresentationService(INominatimService nominatimService, INominatimDtoViewModelMapper mapper) : INominatimPresentationService
{
    public Task<Result<IEnumerable<NominatimSearchResultViewModel>>> SearchAsync(MPoint point, CancellationToken cancellationToken = default)
        => SearchAsync(point.Y, point.Y, cancellationToken);

    public async Task<Result<IEnumerable<NominatimSearchResultViewModel>>> SearchAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        var result = await nominatimService.SearchAsync(latitude, longitude, cancellationToken);
        return result.MapSequence(mapper.MapToViewModel);
    }

    public async Task<Result<IEnumerable<NominatimSearchResultViewModel>>> SearchAsync(string address, CancellationToken cancellationToken = default)
    {
        var result = await nominatimService.SearchAsync(address, cancellationToken);
        return result.MapSequence(mapper.MapToViewModel);
    }
}