using Domain.Models.Dependencies;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Services;

public class SystemPresentationService(ISystemDtoViewModelMapper viewModelMapperMapper, ISystemDtoDomainMapper systemDtoDomainMapper,
    IAccountDtoViewModelMapper accountDtoViewModelMapper, IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
    ISystemService systemService) : ISystemPresentationService
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

    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(CurrencyViewModel currencyViewModel, CancellationToken cancellationToken = default)
    {
        var currencyDto = accountDtoViewModelMapper.MapToDto(currencyViewModel);
        return systemService.GetAllDependenciesAsync(currencyDto, cancellationToken);
    }

    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        var accountDto = accountDtoViewModelMapper.MapToDto(accountViewModel);
        return systemService.GetAllDependenciesAsync(accountDto, cancellationToken);
    }

    public async Task<ColorViewModel> GetRandomColorViewModel(CancellationToken cancellationToken = default)
    {
        var colorDto = await systemService.GetRandomColor(cancellationToken);
        var colorModel = viewModelMapperMapper.MapToViewModel(colorDto);

        return colorModel;
    }

    public async Task<IEnumerable<ColorViewModel>> GetAllColorViewModelAsync(CancellationToken cancellationToken = default)
    {
        var colors = await systemService.GetAllColors(cancellationToken);
        var colorViewModels = colors.Select(viewModelMapperMapper.MapToViewModel);

        return colorViewModels;
    }

    public async Task<PlaceViewModel?> GetPlaceViewModel(int placeId, CancellationToken cancellationToken = default)
    {
        var placeDto = await systemService.GetPlace(placeId, cancellationToken);
        if (placeDto is null) return null;

        var placeViewModel = viewModelMapperMapper.MapToViewModel(placeDto);
        return placeViewModel;
    }
}