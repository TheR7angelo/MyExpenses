using Domain.Models.Systems;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Application.Interfaces.Mappings;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Infrastructure.Mapping;

[Mapper(UseDeepCloning = true)]
public partial class SystemDtoDomainMapper : ISystemDtoDomainMapper
{
    public partial ColorDomain MapToDomain(ColorDto src);

    public partial ColorDto MapToDto(ColorDomain src);

    public partial RecursiveFrequencyDto MapToDto(RecursiveFrequencyDomain src);

    public partial RecursiveFrequencyDomain MapToDomain(RecursiveFrequencyDto src);
}