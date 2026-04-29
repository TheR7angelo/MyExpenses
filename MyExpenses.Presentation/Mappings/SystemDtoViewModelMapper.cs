using Domain.Models.Expenses;
using Domain.Models.Systems;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.ViewModels.Systems;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Presentation.Mappings;

[Mapper(UseDeepCloning = true)]
public partial class SystemDtoViewModelMapper : ISystemDtoViewModelMapper
{
    public partial ColorDto MapToDto(ColorDomain src);

    public partial ColorViewModel MapToViewModel(ColorDto src);

    public partial ColorViewModel Clone(ColorViewModel src);

    [MapperIgnoreSource(nameof(PlaceViewModel.HasErrors))]
    public partial PlaceDto MapToDto(PlaceViewModel src);

    public partial PlaceViewModel MapToViewModel(PlaceDto src);

    public partial RecursiveFrequencyDto MapToDto(RecursiveFrequencyViewModel src);

    public partial RecursiveFrequencyViewModel MapToViewModel(RecursiveFrequencyDto src);

    [MapperIgnoreSource(nameof(ColorViewModel.HasErrors))]
    public partial ColorDto MapToDto(ColorViewModel src);
}