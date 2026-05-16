using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Expenses;
using MyExpenses.Presentation.ViewModels.Expenses;
using CategoryTypeViewModel = MyExpenses.Presentation.ViewModels.Expenses.CategoryTypeViewModel;

namespace MyExpenses.Presentation.Mappings.Interfaces;

public interface IExpenseDtoViewModelMapper
{
    /// <summary>
    /// Maps a <see cref="CategoryTypeDto"/> to a <see cref="CategoryTypeViewModel"/>.
    /// </summary>
    /// <param name="src">
    /// The source <see cref="CategoryTypeDto"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="CategoryTypeViewModel"/> containing the mapped data from the provided <see cref="CategoryTypeDto"/>.
    /// </returns>
    public CategoryTypeViewModel MapToViewModel(CategoryTypeDto src);

    /// <summary>
    /// Maps a <see cref="CategoryTypeViewModel"/> to a <see cref="CategoryTypeDto"/>.
    /// </summary>
    /// <param name="src">
    /// The source <see cref="CategoryTypeViewModel"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="CategoryTypeDto"/> containing the mapped data from the provided <see cref="CategoryTypeViewModel"/>.
    /// </returns>
    public CategoryTypeDto MapToDto(CategoryTypeViewModel src);

    /// <summary>
    /// Creates a copy of the provided <see cref="CategoryTypeViewModel"/> instance.
    /// </summary>
    /// <param name="categoryTypeViewModel">
    /// The <see cref="CategoryTypeViewModel"/> instance to be cloned.
    /// </param>
    /// <returns>
    /// A new <see cref="CategoryTypeViewModel"/> instance that is a copy of the provided instance.
    /// </returns>
    public CategoryTypeViewModel Clone(CategoryTypeViewModel categoryTypeViewModel);

    /// <summary>
    /// Maps a <see cref="HistoryViewModel"/> to a <see cref="HistoryDto"/>.
    /// </summary>
    /// <param name="src">
    /// The source <see cref="HistoryViewModel"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="HistoryDto"/> containing the mapped data from the provided <see cref="HistoryViewModel"/>.
    /// </returns>
    public HistoryDto MapToDto(HistoryViewModel src);

    /// <summary>
    /// Maps a <see cref="HistoryDto"/> to a <see cref="HistoryViewModel"/>.
    /// </summary>
    /// <param name="src">
    /// The source <see cref="HistoryDto"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="HistoryViewModel"/> containing the mapped data from the provided <see cref="HistoryDto"/>.
    /// </returns>
    public HistoryViewModel MapToViewModel(HistoryDto src);

    /// <summary>
    /// Maps a <see cref="ModePaymentViewModel"/> to a <see cref="ModePaymentDto"/>.
    /// </summary>
    /// <param name="src">
    /// The source <see cref="ModePaymentViewModel"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="ModePaymentDto"/> containing the mapped data from the provided <see cref="ModePaymentViewModel"/>.
    /// </returns>
    public ModePaymentDto MapToDto(ModePaymentViewModel src);

    /// <summary>
    /// Maps a <see cref="ModePaymentDto"/> to a <see cref="ModePaymentViewModel"/>.
    /// </summary>
    /// <param name="src">
    /// The source <see cref="ModePaymentDto"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="ModePaymentViewModel"/> containing the mapped data from the provided <see cref="ModePaymentDto"/>.
    /// </returns>
    public ModePaymentViewModel MapToViewModel(ModePaymentDto src);

    /// <summary>
    /// Maps a <see cref="BankTransferViewModel"/> to a <see cref="BankTransferDto"/>.
    /// </summary>
    /// <param name="src">
    /// The source <see cref="BankTransferViewModel"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="BankTransferDto"/> containing the mapped data from the provided <see cref="BankTransferViewModel"/>.
    /// </returns>
    public BankTransferDto MapToDto(BankTransferViewModel src);

    /// <summary>
    /// Maps a <see cref="BankTransferDto"/> to a <see cref="BankTransferViewModel"/>.
    /// </summary>
    /// <param name="src">
    /// The source <see cref="BankTransferDto"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="BankTransferViewModel"/> containing the mapped data from the provided <see cref="BankTransferDto"/>.
    /// </returns>
    public BankTransferViewModel MapToViewModel(BankTransferDto src);

    /// <summary>
    /// Maps a <see cref="RecursiveExpenseViewModel"/> to a <see cref="RecursiveExpenseDto"/>.
    /// </summary>
    /// <param name="src">
    /// The source <see cref="RecursiveExpenseViewModel"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="RecursiveExpenseDto"/> containing the mapped data from the provided <see cref="RecursiveExpenseViewModel"/>.
    /// </returns>
    public RecursiveExpenseDto MapToDto(RecursiveExpenseViewModel src);

    /// <summary>
    /// Maps a <see cref="RecursiveExpenseDto"/> to a <see cref="RecursiveExpenseViewModel"/>.
    /// </summary>
    /// <param name="src">
    /// The source <see cref="RecursiveExpenseDto"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="RecursiveExpenseViewModel"/> containing the mapped data from the provided <see cref="RecursiveExpenseDto"/>.
    /// </returns>
    public RecursiveExpenseViewModel MapToViewModel(RecursiveExpenseDto src);

    /// <summary>
    /// Maps a <see cref="Result{CategoryTypeDto}"/> to a <see cref="Result{CategoryTypeViewModel}"/>.
    /// </summary>
    /// <param name="result">
    /// The source <see cref="Result{CategoryTypeDto}"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="Result{CategoryTypeViewModel}"/> containing the mapped data from the provided <see cref="Result{CategoryTypeDto}"/>.
    /// </returns>
    public Result<CategoryTypeViewModel> MapToViewModel(Result<CategoryTypeDto> result);

    /// <summary>
    /// Maps a <see cref="Result{HistoryDto}"/> to a <see cref="Result{HistoryViewModel}"/>.
    /// </summary>
    /// <param name="result">
    /// The source <see cref="Result{HistoryDto}"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="Result{HistoryViewModel}"/> containing the mapped data from the provided <see cref="Result{HistoryDto}"/>.
    /// </returns>
    public Result<HistoryViewModel> Map(Result<HistoryDto> result);

    /// <summary>
    /// Merges the data from a <see cref="BankTransferViewModel"/> source into a <see cref="HistoryViewModel"/> destination.
    /// </summary>
    /// <param name="src">
    /// The <see cref="BankTransferViewModel"/> containing the source data to be merged.
    /// </param>
    /// <param name="dst">
    /// The <see cref="HistoryViewModel"/> that will receive the merged data from the source.
    /// </param>
    public void Merge(BankTransferViewModel src, HistoryViewModel dst);

    /// <summary>
    /// Merges the properties of the source <see cref="CategoryTypeViewModel"/> into the destination <see cref="CategoryTypeViewModel"/>.
    /// </summary>
    /// <param name="src">The source <see cref="CategoryTypeViewModel"/> containing the properties to merge.</param>
    /// <param name="dst">The destination <see cref="CategoryTypeViewModel"/> to update with merged properties.</param>
    public void Merge(CategoryTypeViewModel src, CategoryTypeViewModel dst);

    /// <summary>
    /// Maps a <see cref="BankTransferDto"/> and a collection of <see cref="HistoryDto"/>
    /// to a tuple containing a <see cref="BankTransferViewModel"/>
    /// and an enumerable of <see cref="HistoryViewModel"/>.
    /// </summary>
    /// <param name="result">
    /// The <see cref="Result{T}"/> containing the tuple of <see cref="BankTransferDto"/>
    /// and <see cref="IEnumerable{T}"/> of <see cref="HistoryDto"/> to be mapped.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a tuple of <see cref="BankTransferViewModel"/>
    /// and <see cref="IEnumerable{T}"/> of <see cref="HistoryViewModel"/>.
    /// </returns>
    public Result<(BankTransferViewModel bankTransferViewModel, IEnumerable<HistoryViewModel> historyViewModel)> MapToViewModel(Result<(BankTransferDto bankTransfer, IEnumerable<HistoryDto> historyDtos)> result);
}