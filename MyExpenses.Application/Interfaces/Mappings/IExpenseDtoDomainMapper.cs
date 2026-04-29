using Domain.Models.Expenses;
using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Expenses;

namespace MyExpenses.Application.Interfaces.Mappings;

public interface IExpenseDtoDomainMapper
{
    /// <summary>
    /// Maps the CategoryTypeDomain object to a CategoryTypeDto object.
    /// </summary>
    /// <param name="src">The source CategoryTypeDomain object to map.</param>
    /// <returns>A CategoryTypeDto object that is mapped from the source object.</returns>
    public CategoryTypeDto MapToDto(CategoryTypeDomain src);

    /// <summary>
    /// Maps the CategoryTypeDto object to a CategoryTypeDomain object.
    /// </summary>
    /// <param name="src">The source CategoryTypeDto object to map.</param>
    /// <returns>A CategoryTypeDomain object that is mapped from the source object.</returns>
    public CategoryTypeDomain MapToDomain(CategoryTypeDto src);

    /// <summary>
    /// Maps the HistoryDto object to a HistoryDomain object.
    /// </summary>
    /// <param name="historyDto">The source HistoryDto object to map.</param>
    /// <returns>A HistoryDomain object that is mapped from the source HistoryDto object.</returns>
    public HistoryDomain MapToDomain(HistoryDto historyDto);

    /// <summary>
    /// Maps the HistoryDomain object to a HistoryDto object.
    /// </summary>
    /// <param name="src">The source HistoryDomain object to map.</param>
    /// <returns>A HistoryDto object that is mapped from the source object.</returns>
    public HistoryDto MapToDto(HistoryDomain src);

    /// <summary>
    /// Maps the ModePaymentDto object to a ModePaymentDomain object.
    /// </summary>
    /// <param name="src">The source ModePaymentDto object to map.</param>
    /// <returns>A ModePaymentDomain object that is mapped from the source object.</returns>
    public ModePaymentDomain MapToDomain(ModePaymentDto src);

    /// <summary>
    /// Maps the ModePaymentDomain object to a ModePaymentDto object.
    /// </summary>
    /// <param name="src">The source ModePaymentDomain object to map.</param>
    /// <returns>A ModePaymentDto object that is mapped from the source object.</returns>
    public ModePaymentDto MapToDto(ModePaymentDomain src);

    /// <summary>
    /// Maps the BankTransferDto object to a BankTransferDomain object.
    /// </summary>
    /// <param name="src">The source BankTransferDto object to map.</param>
    /// <returns>A BankTransferDomain object that is mapped from the source object.</returns>
    public BankTransferDomain MapToDomain(BankTransferDto src);

    /// <summary>
    /// Maps the BankTransferDomain object to a BankTransferDto object.
    /// </summary>
    /// <param name="src">The source BankTransferDomain object to map.</param>
    /// <returns>A BankTransferDto object that is mapped from the source object.</returns>
    public BankTransferDto MapToDto(BankTransferDomain src);

    /// <summary>
    /// Maps the RecursiveExpenseDto object to a RecursiveExpenseDomain object.
    /// </summary>
    /// <param name="src">The source RecursiveExpenseDto object to map.</param>
    /// <returns>A RecursiveExpenseDomain object that is mapped from the source object.</returns>
    public RecursiveExpenseDomain MapToDomain(RecursiveExpenseDto src);

    /// <summary>
    /// Maps the RecursiveExpenseDomain object to a RecursiveExpenseDto object.
    /// </summary>
    /// <param name="src">The RecursiveExpenseDomain object to map.</param>
    /// <returns>A RecursiveExpenseDto object that is mapped from the source RecursiveExpenseDomain object.</returns>
    public RecursiveExpenseDto MapToDto(RecursiveExpenseDomain src);

    /// <summary>
    /// Maps a Result containing a HistoryDomain object to a Result containing a HistoryDto object.
    /// </summary>
    /// <param name="result">The Result object containing a HistoryDomain to map.</param>
    /// <returns>A Result object containing a mapped HistoryDto object.</returns>
    public Result<HistoryDto> Map(Result<HistoryDomain> result);

    /// <summary>
    /// Maps a Result object containing a CategoryTypeDomain to a Result object containing a CategoryTypeDto.
    /// </summary>
    /// <param name="categoryTypeDomain">
    /// The Result object containing a CategoryTypeDomain to map.
    /// </param>
    /// <returns>
    /// A Result object containing a CategoryTypeDto that is mapped from the source object.
    /// </returns>
    public Result<CategoryTypeDto> Map(Result<CategoryTypeDomain> categoryTypeDomain);
}