using Domain.Models.Systems;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.ViewModels.Systems;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Presentation.Mappings;

[Mapper]
public partial class SystemDtoViewModel : ISystemDtoViewModel
{
    public partial ColorDto MapToDto(ColorDomain src);

    public partial ColorViewModel MapToViewModel(ColorDto src);
}