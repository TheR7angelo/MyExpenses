using Domain.Models.Dependencies;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Services;

public class SystemPresentationService(ISystemDtoViewModelMapper viewModelMapperMapper,
    IAccountDtoViewModelMapper accountDtoViewModelMapper, IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
    ISystemService systemService,
    ISystemRepository systemRepository) : ISystemPresentationService
{
    public async Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(AccountTypeViewModel accountTypeViewModel,
        CancellationToken cancellationToken = default)
    {
        var accountTypeDto = accountDtoViewModelMapper.MapToDto(accountTypeViewModel);
        return await systemService.GetAllDependenciesAsync(accountTypeDto, cancellationToken);
    }

    public async Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default)
    {
        var categoryTypeDto = expenseDtoViewModelMapper.MapToDto(categoryTypeViewModel);
        return await systemService.GetAllDependenciesAsync(categoryTypeDto, cancellationToken);
    }

    public async Task<ColorViewModel> GetRandomColorViewModel(CancellationToken cancellationToken = default)
    {
        var randomColor = await systemRepository.GetRandomColor(cancellationToken);
        var colorDto = viewModelMapperMapper.MapToDto(randomColor);
        var colorModel = viewModelMapperMapper.MapToViewModel(colorDto);

        return colorModel;
    }


}