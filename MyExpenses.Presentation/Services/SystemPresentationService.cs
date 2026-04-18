using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Services;

public class SystemPresentationService(ISystemDtoViewModel viewModelMapper,
    ISystemRepository systemRepository) : ISystemPresentationService
{
    public async Task<ColorViewModel> GetRandomColorViewModel(CancellationToken cancellationToken = default)
    {
        var randomColor = await systemRepository.GetRandomColor(cancellationToken);
        var colorDto = viewModelMapper.MapToDto(randomColor);
        var colorModel = viewModelMapper.MapToViewModel(colorDto);

        return colorModel;
    }
}