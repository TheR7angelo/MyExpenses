using Domain.Models.Systems;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Mappings.Interfaces;

public interface ISystemDtoViewModel
{
    public ColorDto MapToDto(ColorDomain src);

    public ColorViewModel MapToViewModel(ColorDto src);
}