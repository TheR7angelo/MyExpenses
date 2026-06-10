using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Expenses;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;

namespace MyExpenses.Infrastructure.Services;

public class ExpenseService(IExpenseRepository expenseRepository, IExpenseDtoDomainMapper mapper) : IExpenseService
{
    public async Task<Result<IEnumerable<CategoryTypeDto>>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default)
    {
        var result = await expenseRepository.GetAllCategoryTypesAsync(cancellationToken);
        return result.MapSequence(mapper.MapToDto);
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

    public async Task<Result<HistoryDto>> UpdateExpenseAsync(HistoryDto historyDto, CancellationToken cancellationToken = default)
    {
        var domain = mapper.MapToDomain(historyDto);
        var result = await expenseRepository.UpdateExpenseAsync(domain, cancellationToken);
        return mapper.Map(result);
    }

    public Task<DeletionResult> DeleteHistoryAsync(HistoryDto historyDto, CancellationToken cancellationToken = default)
    {
        var domain = mapper.MapToDomain(historyDto);
        return expenseRepository.DeleteHistoryAsync(domain, cancellationToken);
    }

    public async Task<Result<IEnumerable<ModePaymentDto>>> GetAllModePaymentAsync(CancellationToken cancellationToken = default)
    {
        var result = await expenseRepository.GetAllModePaymentAsync(cancellationToken);
        return result.MapSequence(mapper.MapToDto);
    }

    public async Task<ModePaymentDto?> GetModePaymentByIdAsync(int modePaymentId, CancellationToken cancellationToken = default)
    {
        var modePayment = await expenseRepository.GetModePaymentByIdAsync(modePaymentId, cancellationToken);
        return modePayment is null ? null : mapper.MapToDto(modePayment);
    }

    public async Task<Result<(BankTransferDto bankTransfer, IEnumerable<HistoryDto> historyDtos)>> CreateBankTransferAsync(BankTransferDto bankTransferDto, HistoryDto historyDto,
        CancellationToken cancellationToken = default)
    {
        var bankTransferDomain = mapper.MapToDomain(bankTransferDto);
        var historyDomain = mapper.MapToDomain(historyDto);
        var result = await expenseRepository.CreateBankTransferAsync(bankTransferDomain, historyDomain, cancellationToken);
        return mapper.Map(result);
    }

    public async Task<Result<ModePaymentDto>> CreateModePaymentAsync(ModePaymentDto modePaymentDto, CancellationToken cancellationToken = default)
    {
        var modePaymentDomain = mapper.MapToDomain(modePaymentDto);
        var result = await expenseRepository.CreateModePaymentAsync(modePaymentDomain, cancellationToken);
        return result.Map(mapper.MapToDto);
    }

    public async Task<Result<ModePaymentDto>> UpdateModePaymentAsync(ModePaymentDto modePaymentDto, CancellationToken cancellationToken = default)
    {
        var modePaymentDomain = mapper.MapToDomain(modePaymentDto);
        var result = await expenseRepository.UpdateModePaymentAsync(modePaymentDomain, cancellationToken);
        return result.Map(mapper.MapToDto);
    }

    public Task<DeletionResult> DeleteModePaymentAsync(ModePaymentDto modePaymentDto, CancellationToken cancellationToken = default)
    {
        var modePaymentDomain = mapper.MapToDomain(modePaymentDto);
        return expenseRepository.DeleteModePaymentAsync(modePaymentDomain, cancellationToken);
    }
}