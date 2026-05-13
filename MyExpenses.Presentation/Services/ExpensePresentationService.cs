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
    public async Task<IEnumerable<CategoryTypeViewModel>> GetAllCategoryTypeViewModelAsync(CancellationToken cancellationToken = default)
    {
        var categoryTypes = await expenseService.GetAllCategoryTypesAsync(cancellationToken);
        return categoryTypes.Select(mapper.MapToViewModel);
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
        var historyDto = mapper.MapToDto(historyViewModel);
        var result = await expenseService.CreateExpenseAsync(historyDto, cancellationToken);
        return mapper.Map(result);
    }

    public async Task<IEnumerable<ModePaymentViewModel>> GetAllModePaymentViewModelAsync(CancellationToken cancellationToken = default)
    {
        var modePayment = await expenseService.GetAllModePaymentAsync(cancellationToken);
        return modePayment.Select(mapper.MapToViewModel);
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

    public async Task<Result<CategoryTypeViewModel>> CreateCategoryType(CategoryTypeViewModel newCategoryType, CancellationToken cancellationToken = default)
    {
        var categoryTypeDto = mapper.MapToDto(newCategoryType);
        var result = await expenseService.CreateCategoryTypeAsync(categoryTypeDto, cancellationToken);
        return mapper.MapToViewModel(result);
    }
}