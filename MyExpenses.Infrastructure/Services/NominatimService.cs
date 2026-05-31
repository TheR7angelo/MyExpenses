using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Nominatium;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;

namespace MyExpenses.Infrastructure.Services;

public class NominatimService(INominatimRepository nominatimRepository, INominatimDtoDomainMapper mapper) : INominatimService
{
    public async Task<Result<IEnumerable<NominatimSearchResultDto>>> SearchAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        var result = await nominatimRepository.SearchAsync(latitude, longitude, cancellationToken);
        return result.MapSequence(mapper.MapToDto);
    }

    public async Task<Result<IEnumerable<NominatimSearchResultDto>>> SearchAsync(string address, CancellationToken cancellationToken = default)
    {
        var result = await nominatimRepository.SearchAsync(address, cancellationToken);
        return result.MapSequence(mapper.MapToDto);
    }
}