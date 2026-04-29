using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Expenses;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;

namespace MyExpenses.Infrastructure.Services;

public class ExpenseService(IExpenseRepository expenseRepository, IExpenseDtoDomainMapper mapper) : IExpenseService
{
    public async Task<IEnumerable<CategoryTypeDto>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await expenseRepository.GetAllCategoryTypesAsync(cancellationToken);
        return categories.Select(mapper.MapToDto);
    }

    public async Task<Result<CategoryTypeDto>> CreateCategoryTypeAsync(CategoryTypeDto categoryTypeDto, CancellationToken cancellationToken = default)
    {
        var categoryType = mapper.MapToDomain(categoryTypeDto);
        var result = await expenseRepository.CreateCategoryTypeAsync(categoryType, cancellationToken);
        return mapper.Map(result);
    }

    public async Task<DeletionResult> DeleteCategoryTypeAsync(CategoryTypeDto categoryTypeDto, CancellationToken cancellationToken = default)
    {
        var categoryType = mapper.MapToDomain(categoryTypeDto);
        return await expenseRepository.DeleteCategoryTypeAsync(categoryType, cancellationToken);
    }

    public async Task<Result> UpdateCategoryTypeNameAsync(CategoryTypeDto categoryTypeDto, CancellationToken cancellationToken = default)
    {
        var categoryType = mapper.MapToDomain(categoryTypeDto);
        return await expenseRepository.UpdateCategoryTypeNameAsync(categoryType, cancellationToken);
    }

    public async Task<Result<HistoryDto>> CreateExpenseAsync(HistoryDto historyDto, CancellationToken cancellationToken = default)
    {
        var domain = mapper.MapToDomain(historyDto);
        var result = await expenseRepository.CreateExpenseAsync(domain, cancellationToken);
        return mapper.Map(result);
    }

    public async Task<ModePaymentDto?> GetModePaymentByIdAsync(int modePaymentId, CancellationToken cancellationToken = default)
    {
        var modePayment = await expenseRepository.GetModePaymentByIdAsync(modePaymentId, cancellationToken);
        return modePayment is null ? null : mapper.MapToDto(modePayment);
    }
}