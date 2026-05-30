using Domain.Models.Nominatim;
using MyExpenses.Application.Dtos.Nominatium;
using MyExpenses.Application.Interfaces.Mappings;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Infrastructure.Mapping;

[Mapper]
public partial class NominatimDtoDomainMapper : INominatimDtoDomainMapper
{
    public partial NominatimSearchResultDto MapToDto(NominatimSearchResultDomain domain);

    public partial NominatimDetailedAddressDto MapToDto(NominatimDetailedAddressDomain domain);

    public partial NominatiumGeoJsonDto MapToDto(NominatimGeoJsonDomain domain);
}