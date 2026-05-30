using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Nominatium;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;

namespace MyExpenses.Infrastructure.Services;

public class NominatiumService(INominatimRepository nominatimRepository) : INominatiumService
{
    public async Task<Result<IEnumerable<NominatimSearchResultDto>>> SearchAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        var result = await nominatimRepository.SearchAsync(latitude, longitude, cancellationToken);
        // TODO continue

        throw new NotImplementedException();
    }
}