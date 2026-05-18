using Domain.Models.Systems;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.ViewModels.Systems;
using Riok.Mapperly.Abstractions;
using PlaceViewModel = MyExpenses.Presentation.ViewModels.Locations.PlaceViewModel;

namespace MyExpenses.Presentation.Mappings;

[Mapper(UseDeepCloning = true)]
public partial class SystemDtoViewModelMapper : ISystemDtoViewModelMapper
{
    public partial ColorDto MapToDto(ColorDomain src);

    [MapperIgnoreSource(nameof(ColorViewModel.HasErrors))]
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public partial ColorDto MapToDto(ColorViewModel src);

    public partial ColorViewModel MapToViewModel(ColorDto src);

    public partial ColorViewModel Clone(ColorViewModel src);

    public partial RecursiveFrequencyDto MapToDto(RecursiveFrequencyViewModel src);

    public partial RecursiveFrequencyViewModel MapToViewModel(RecursiveFrequencyDto src);

    public partial void Merge(ColorViewModel src, ColorViewModel dst);
}