using Domain.Models.Systems;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Application.Interfaces.Mappings;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Infrastructure.Mapping;

[Mapper(UseDeepCloning = true)]
public partial class LocationDtoDomainMapper : ILocationDtoDomainMapper
{
    public partial PlaceDto MapToDto(PlaceDomain src);

    public partial PlaceDomain MapToDomain(PlaceDto src);
}