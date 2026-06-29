using Domain.Models.Dependencies;
using Domain.Models.Validation;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Locations;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Services;

public class SystemPresentationService(ISystemDtoViewModelMapper viewModelMapperMapper,
    IAccountDtoViewModelMapper accountDtoViewModelMapper, IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
    ILocationDtoViewModelMapper locationDtoViewModelMapper,
    ISystemDtoViewModelMapper systemDtoViewModelMapper,
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

    public Task<Result<IEnumerable<DeletionDependency>>> GetAllDependenciesAsync(ColorViewModel colorViewModel, CancellationToken cancellationToken = default)
    {
        var colorDto = systemDtoViewModelMapper.MapToDto(colorViewModel);
        return systemService.GetAllDependenciesAsync(colorDto, cancellationToken);
    }

    public Task<Result<IEnumerable<DeletionDependency>>> GetAllDependenciesAsync(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default)
    {
        var placeDto = locationDtoViewModelMapper.MapToDto(placeViewModel);
        return systemService.GetAllDependenciesAsync(placeDto, cancellationToken);
    }

    public Task<Result<IEnumerable<DeletionDependency>>> GetAllDependenciesAsync(ModePaymentViewModel modePaymentViewModel, CancellationToken cancellationToken = default)
    {
        var modePaymentDto = expenseDtoViewModelMapper.MapToDto(modePaymentViewModel);
        return systemService.GetAllDependenciesAsync(modePaymentDto, cancellationToken);
    }

    public Task<Result<IEnumerable<DeletionDependency>>> GetAllDependenciesAsync(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default)
    {
        var historyDto = expenseDtoViewModelMapper.MapToDto(historyViewModel);
        return systemService.GetAllDependenciesAsync(historyDto, cancellationToken);
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

    public async Task<Result<ColorViewModel>> CreateColorAsync(ColorViewModel colorViewModel, CancellationToken cancellationToken = default)
    {
        var colorDto = viewModelMapperMapper.MapToDto(colorViewModel);
        var result = await systemService.CreateColorAsync(colorDto, cancellationToken);

        return result.Map(viewModelMapperMapper.MapToViewModel);
    }

    public async Task<Result<ColorViewModel>> UpdateColorAsync(ColorViewModel colorViewModel, CancellationToken cancellationToken = default)
    {
        var colorDto = viewModelMapperMapper.MapToDto(colorViewModel);
        var result = await systemService.UpdateColorAsync(colorDto, cancellationToken);

        return result.Map(viewModelMapperMapper.MapToViewModel);
    }

    public Task<DeletionResult> DeleteColorAsync(ColorViewModel colorViewModel, CancellationToken cancellationToken = default)
    {
        var colorDto = viewModelMapperMapper.MapToDto(colorViewModel);
        return systemService.DeleteColorAsync(colorDto, cancellationToken);
    }

    public async Task<Result<IEnumerable<RecursiveFrequencyViewModel>>> GetAllFrequencyViewModelAsync(CancellationToken cancellationToken = default)
    {
        var result = await systemService.GetAllFrequencyDtoAsync(cancellationToken);
        return result.MapSequence(systemDtoViewModelMapper.MapToViewModel);
    }
}