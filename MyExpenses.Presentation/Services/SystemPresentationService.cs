using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Services;

public class SystemPresentationService(ISystemDtoViewModelMapper viewModelMapperMapper,
    ISystemRepository systemRepository) : ISystemPresentationService
{
    public async Task<ColorViewModel> GetRandomColorViewModel(CancellationToken cancellationToken = default)
    {
        var randomColor = await systemRepository.GetRandomColor(cancellationToken);
        var colorDto = viewModelMapperMapper.MapToDto(randomColor);
        var colorModel = viewModelMapperMapper.MapToViewModel(colorDto);

        return colorModel;
    }
}