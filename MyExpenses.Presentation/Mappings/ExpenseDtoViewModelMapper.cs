using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Dtos.Analysis;
using MyExpenses.Application.Dtos.Expenses;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Analysis;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Systems;
using Riok.Mapperly.Abstractions;
using PlaceViewModel = MyExpenses.Presentation.ViewModels.Locations.PlaceViewModel;

namespace MyExpenses.Presentation.Mappings;

[Mapper(UseDeepCloning = true)]
public partial class ExpenseDtoViewModelMapper(IAccountDtoViewModelMapper accountDtoViewModelMapper,
    ISystemDtoViewModelMapper mapper, ILocationDtoViewModelMapper locationDtoViewModelMapper) : IExpenseDtoViewModelMapper
{
    public partial CategoryTypeViewModel MapToViewModel(CategoryTypeDto src);

    [MapperIgnoreSource(nameof(CategoryTypeViewModel.HasErrors))]
    public partial CategoryTypeDto MapToDto(CategoryTypeViewModel src);

    public partial CategoryTypeViewModel Clone(CategoryTypeViewModel categoryTypeViewModel);

    [MapProperty(nameof(HistoryViewModel.AccountViewModel), nameof(HistoryDto.Account))]
    [MapProperty(nameof(HistoryViewModel.CategoryTypeViewModel), nameof(HistoryDto.CategoryType))]
    [MapProperty(nameof(HistoryViewModel.PlaceViewModel), nameof(HistoryDto.Place))]
    [MapProperty(nameof(HistoryViewModel.ModePaymentViewModel), nameof(HistoryDto.ModePayment))]
    [MapProperty(nameof(HistoryViewModel.BankTransferViewModel), nameof(HistoryDto.BankTransfer))]
    [MapProperty(nameof(HistoryViewModel.RecursiveExpenseViewModel), nameof(HistoryDto.RecursiveExpense))]
    [MapperIgnoreSource(nameof(HistoryViewModel.HasErrors))]
    public partial HistoryDto MapToDto(HistoryViewModel src);

    [MapProperty(nameof(HistoryDto.Account), nameof(HistoryViewModel.AccountViewModel))]
    [MapProperty(nameof(HistoryDto.CategoryType), nameof(HistoryViewModel.CategoryTypeViewModel))]
    [MapProperty(nameof(HistoryDto.Place), nameof(HistoryViewModel.PlaceViewModel))]
    [MapProperty(nameof(HistoryDto.ModePayment), nameof(HistoryViewModel.ModePaymentViewModel))]
    [MapProperty(nameof(HistoryDto.BankTransfer), nameof(HistoryViewModel.BankTransferViewModel))]
    [MapProperty(nameof(HistoryDto.RecursiveExpense), nameof(HistoryViewModel.RecursiveExpenseViewModel))]
    public partial HistoryViewModel MapToViewModel(HistoryDto src);

    [MapperIgnoreSource(nameof(ModePaymentViewModel.HasErrors))]
    public partial ModePaymentDto MapToDto(ModePaymentViewModel src);

    public partial ModePaymentViewModel MapToViewModel(ModePaymentDto src);

    [MapperIgnoreSource(nameof(BankTransferViewModel.HasErrors))]
    public partial BankTransferDto MapToDto(BankTransferViewModel src);

    public partial BankTransferViewModel MapToViewModel(BankTransferDto src);

    [MapProperty(nameof(RecursiveExpenseViewModel.AccountViewModel), nameof(RecursiveExpenseDto.Account))]
    [MapProperty(nameof(RecursiveExpenseViewModel.CategoryTypeViewModel), nameof(RecursiveExpenseDto.CategoryType))]
    [MapProperty(nameof(RecursiveExpenseViewModel.ModePaymentViewModel), nameof(RecursiveExpenseDto.ModePayment))]
    [MapProperty(nameof(RecursiveExpenseViewModel.RecursiveFrequencyViewModel), nameof(RecursiveExpenseDto.RecursiveFrequency))]
    [MapProperty(nameof(RecursiveExpenseViewModel.PlaceViewModel), nameof(RecursiveExpenseDto.Place))]
    [MapperIgnoreSource(nameof(RecursiveExpenseViewModel.HasErrors))]
    public partial RecursiveExpenseDto MapToDto(RecursiveExpenseViewModel src);

    [MapProperty(nameof(RecursiveExpenseDto.Account), nameof(RecursiveExpenseViewModel.AccountViewModel))]
    [MapProperty(nameof(RecursiveExpenseDto.CategoryType), nameof(RecursiveExpenseViewModel.CategoryTypeViewModel))]
    [MapProperty(nameof(RecursiveExpenseDto.ModePayment), nameof(RecursiveExpenseViewModel.ModePaymentViewModel))]
    [MapProperty(nameof(RecursiveExpenseDto.RecursiveFrequency), nameof(RecursiveExpenseViewModel.RecursiveFrequencyViewModel))]
    [MapProperty(nameof(RecursiveExpenseDto.Place), nameof(RecursiveExpenseViewModel.PlaceViewModel))]
    public partial RecursiveExpenseViewModel MapToViewModel(RecursiveExpenseDto src);

    public Result<CategoryTypeViewModel> MapToViewModel(Result<CategoryTypeDto> result)
        => result.Map(MapToViewModel);

    public Result<HistoryViewModel> Map(Result<HistoryDto> result)
        => result.Map(MapToViewModel);

    [MapperIgnoreSource(nameof(BankTransferViewModel.HasErrors))]
    [MapperIgnoreSource(nameof(BankTransferViewModel.Id))]
    [MapperIgnoreSource(nameof(BankTransferViewModel.FromAccount))]
    [MapperIgnoreSource(nameof(BankTransferViewModel.ToAccount))]
    [MapperIgnoreSource(nameof(BankTransferViewModel.AdditionalReason))]
    [MapperIgnoreTarget(nameof(HistoryViewModel.HasErrors))]
    [MapperIgnoreTarget(nameof(HistoryViewModel.Id))]
    [MapperIgnoreTarget(nameof(HistoryViewModel.AccountViewModel))]
    [MapperIgnoreTarget(nameof(HistoryViewModel.CategoryTypeViewModel))]
    [MapperIgnoreTarget(nameof(HistoryViewModel.ModePaymentViewModel))]
    [MapperIgnoreTarget(nameof(HistoryViewModel.PlaceViewModel))]
    [MapperIgnoreTarget(nameof(HistoryViewModel.RecursiveExpenseViewModel))]
    [MapperIgnoreTarget(nameof(HistoryViewModel.DatePointed))]
    [MapperIgnoreTarget(nameof(HistoryViewModel.IsPointed))]
    [MapProperty(nameof(BankTransferViewModel.MainReason), nameof(HistoryViewModel.Description))]
    [MapProperty(nameof(BankTransferViewModel), nameof(HistoryViewModel.BankTransferViewModel))]
    public partial void Merge(BankTransferViewModel src, HistoryViewModel dst);

    public partial void Merge(CategoryTypeViewModel src, CategoryTypeViewModel dst);

    public partial void Merge(ModePaymentViewModel src, ModePaymentViewModel dst);

    public partial void Merge(HistoryViewModel src, HistoryViewModel dst);

    public Result<(BankTransferViewModel bankTransferViewModel, IEnumerable<HistoryViewModel> historyViewModel)> MapToViewModel(Result<(BankTransferDto bankTransfer, IEnumerable<HistoryDto> historyDtos)> result)
    {
        var bankTransferDto = MapToViewModel(result.Value.bankTransfer);
        var expenseDtos = result.Value.historyDtos.Select(MapToViewModel);

        return !result.IsSuccess
            ? Result<(BankTransferViewModel, IEnumerable<HistoryViewModel>)>.Failure(result.ErrorCode, result.InternalMessage ?? string.Empty)
            : Result<(BankTransferViewModel, IEnumerable<HistoryViewModel>)>.Success((bankTransferDto, expenseDtos), result.InternalMessage ?? string.Empty);
    }

    public partial ModePaymentViewModel Clone(ModePaymentViewModel src);

    public partial DetailTotalCategoryViewModel MapToViewModel(DetailTotalCategoryDto src);

    private PlaceDto MapToDto(PlaceViewModel source)
        => locationDtoViewModelMapper.MapToDto(source);

    private PlaceViewModel MapToViewModel(PlaceDto source)
        => locationDtoViewModelMapper.MapToViewModel(source);

    private AccountDto MapToDto(AccountViewModel source)
        => accountDtoViewModelMapper.MapToDto(source);

    private AccountViewModel MapToViewModel(AccountDto source)
        => accountDtoViewModelMapper.MapToViewModel(source);

    private RecursiveFrequencyDto MapToDto(RecursiveFrequencyViewModel source)
        => mapper.MapToDto(source);

    private RecursiveFrequencyViewModel MapToViewModel(RecursiveFrequencyDto source)
        => mapper.MapToViewModel(source);

    private ColorDto MapToDto(ColorViewModel source)
        => mapper.MapToDto(source);

    private ColorViewModel MapToViewModel(ColorDto source)
        => mapper.MapToViewModel(source);

    private ColorViewModel Clone(ColorViewModel source)
        => mapper.Clone(source);
}