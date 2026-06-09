using Domain.Models.Validation;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Services;

public class ExpensePresentationService(IExpenseService expenseService, IExpenseDtoViewModelMapper mapper,
    IExpenseValidationRepository expenseValidationRepository,
    ILogger<ExpensePresentationService> logger) : IExpensePresentationService
{
    public async Task<Result<IEnumerable<CategoryTypeViewModel>>> GetAllCategoryTypeViewModelAsync(CancellationToken cancellationToken = default)
    {
        var result = await expenseService.GetAllCategoryTypesAsync(cancellationToken);
        return result.MapSequence(mapper.MapToViewModel);
    }

    public async Task<bool> IsCategoryTypeNameAvailableAsync(string input, CancellationToken cancellationToken = default)
    {
        using var scope = logger.BeginScope("Checking category type name availability. Input={Input}", input);

        logger.LogInformation("Starting validation for category type name availability");

        try
        {
            var alreadyExists = await expenseValidationRepository.IsCategoryTypeNameAlreadyExistAsync(
                input, cancellationToken);

            if (alreadyExists)
            {
                logger.LogInformation("Category type name is already used");
                return false;
            }

            logger.LogInformation("Category type name is available");
            return true;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Validation was canceled");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while checking category type name availability");
            throw;
        }
    }

    public Task<DeletionResult> DeleteCategoryTypeAsync(CategoryTypeViewModel categoryTypeViewModel,
        CancellationToken cancellationToken = default)
    {
        var categoryTypeDto = mapper.MapToDto(categoryTypeViewModel);
        return expenseService.DeleteCategoryTypeAsync(categoryTypeDto, cancellationToken);
    }

    public Task<Result> UpdateCategoryTypeName(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default)
    {
        var categoryTypeDto = mapper.MapToDto(categoryTypeViewModel);
        return expenseService.UpdateCategoryTypeNameAsync(categoryTypeDto, cancellationToken);
    }

    public async Task<Result<HistoryViewModel>> CreateExpense(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default)
    {
        historyViewModel.DateAdded ??= DateTime.Now;
        if (historyViewModel.IsPointed) historyViewModel.DatePointed ??= DateTime.Now;

        var historyDto = mapper.MapToDto(historyViewModel);
        var result = await expenseService.CreateExpenseAsync(historyDto, cancellationToken);
        return mapper.Map(result);
    }

    public async Task<Result<HistoryViewModel>> UpdateExpense(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default)
    {
        var historyDto = mapper.MapToDto(historyViewModel);
        var result = await expenseService.UpdateExpenseAsync(historyDto, cancellationToken);
        return mapper.Map(result);
    }

    public Task<DeletionResult> DeleteHistory(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default)
    {
        // TODO work
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<ModePaymentViewModel>>> GetAllModePaymentViewModelAsync(CancellationToken cancellationToken = default)
    {
        var result = await expenseService.GetAllModePaymentAsync(cancellationToken);
        return result.MapSequence(mapper.MapToViewModel);
    }

    public async Task<ModePaymentViewModel?> GetModePaymentViewModel(int modePaymentId, CancellationToken cancellationToken = default)
    {
        var modePaymentDto = await expenseService.GetModePaymentByIdAsync(modePaymentId, cancellationToken);
        return modePaymentDto is null ? null : mapper.MapToViewModel(modePaymentDto);
    }

    public void Merge(BankTransferViewModel src, HistoryViewModel dst)
    {
        mapper.Merge(src, dst);
    }

    public async Task<Result<(BankTransferViewModel bankTransferViewModel, IEnumerable<HistoryViewModel> historyViewModel)>> CreateBankTransferAsync(BankTransferViewModel bankTransferViewModel, HistoryViewModel historyViewModel,
        CancellationToken cancellationToken = default)
    {
        var bankTransferDto = mapper.MapToDto(bankTransferViewModel);
        var historyDto = mapper.MapToDto(historyViewModel);
        var result = await expenseService.CreateBankTransferAsync(bankTransferDto, historyDto, cancellationToken);

        return mapper.MapToViewModel(result);
    }

    public async Task<Result<CategoryTypeViewModel>> CreateCategoryType(CategoryTypeViewModel newCategoryType, CancellationToken cancellationToken = default)
    {
        var categoryTypeDto = mapper.MapToDto(newCategoryType);
        var result = await expenseService.CreateCategoryTypeAsync(categoryTypeDto, cancellationToken);
        return mapper.MapToViewModel(result);
    }

    public async Task<Result<ModePaymentViewModel>> CreateModePayment(ModePaymentViewModel newModePayment, CancellationToken cancellationToken = default)
    {
        var modePaymentDto = mapper.MapToDto(newModePayment);
        var result = await expenseService.CreateModePaymentAsync(modePaymentDto, cancellationToken);
        return result.Map(mapper.MapToViewModel);
    }

    public async Task<Result<ModePaymentViewModel>> UpdateModePayment(ModePaymentViewModel modePaymentViewModel, CancellationToken cancellationToken = default)
    {
        var modePaymentDto = mapper.MapToDto(modePaymentViewModel);
        var result = await expenseService.UpdateModePaymentAsync(modePaymentDto, cancellationToken);
        return result.Map(mapper.MapToViewModel);
    }

    public Task<DeletionResult> DeleteModePaymentAsync(ModePaymentViewModel modePaymentViewModel, CancellationToken cancellationToken = default)
    {
        var modePaymentDto = mapper.MapToDto(modePaymentViewModel);
        return expenseService.DeleteModePaymentAsync(modePaymentDto, cancellationToken);
    }
}