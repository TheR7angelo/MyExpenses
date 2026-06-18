using Domain.Models.Accounts;
using Domain.Models.Analysis;
using Domain.Models.Expenses;
using Domain.Models.Systems;
using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Dtos.Analysis;
using MyExpenses.Application.Dtos.Expenses;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Application.Interfaces.Mappings;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Infrastructure.Mapping;

[Mapper]
public partial class ExpenseDtoDomainMapper(ISystemDtoDomainMapper systemDtoDomainMapper,
    ILocationDtoDomainMapper locationDtoDomainMapper,
    IAccountDtoDomainMapper accountDtoDomainMapper) : IExpenseDtoDomainMapper
{
    public partial CategoryTypeDto MapToDto(CategoryTypeDomain src);

    public partial CategoryTypeDomain MapToDomain(CategoryTypeDto src);

    public partial HistoryDomain MapToDomain(HistoryDto historyDto);

    public partial HistoryDto MapToDto(HistoryDomain src);

    public partial ModePaymentDomain MapToDomain(ModePaymentDto src);

    public partial ModePaymentDto MapToDto(ModePaymentDomain src);

    public partial BankTransferDomain MapToDomain(BankTransferDto src);

    public partial BankTransferDto MapToDto(BankTransferDomain src);

    public partial RecursiveExpenseDomain MapToDomain(RecursiveExpenseDto src);

    public partial RecursiveExpenseDto MapToDto(RecursiveExpenseDomain src);

    public Result<HistoryDto> Map(Result<HistoryDomain> result)
        => result.Map(MapToDto);

    private PlaceDomain MapToDomain(PlaceDto src)
        => locationDtoDomainMapper.MapToDomain(src);

    private PlaceDto MapToDto(PlaceDomain src)
        => locationDtoDomainMapper.MapToDto(src);

    private RecursiveFrequencyDto MapToDto(RecursiveFrequencyDomain src)
        => systemDtoDomainMapper.MapToDto(src);

    private RecursiveFrequencyDomain MapToDomain(RecursiveFrequencyDto src)
        => systemDtoDomainMapper.MapToDomain(src);

    private AccountDto MapToDto(AccountDomain src)
        => accountDtoDomainMapper.MapToDto(src);

    private AccountDomain MapToDomain(AccountDto src)
        => accountDtoDomainMapper.MapToDomain(src);

    private ColorDomain MapToDomain(ColorDto src)
        => systemDtoDomainMapper.MapToDomain(src);

    private ColorDto MapToDto(ColorDomain src)
        => systemDtoDomainMapper.MapToDto(src);

    public Result<CategoryTypeDto> Map(Result<CategoryTypeDomain> categoryTypeDto)
        => categoryTypeDto.Map(MapToDto);

    public Result<(BankTransferDto bankTransfer, IEnumerable<HistoryDto> historyDtos)> Map(Result<(BankTransferDomain bankTransferDomain, IEnumerable<HistoryDomain> historiesDomain)> historiesDomain)
    {
        var bankTransferDto = MapToDto(historiesDomain.Value.bankTransferDomain);
        var historyDtos = historiesDomain.Value.historiesDomain.Select(MapToDto);

        return !historiesDomain.IsSuccess
            ? Result<(BankTransferDto, IEnumerable<HistoryDto>)>.Failure(historiesDomain.ErrorCode, historiesDomain.InternalMessage ?? string.Empty)
            : Result<(BankTransferDto, IEnumerable<HistoryDto>)>.Success((bankTransferDto, historyDtos), historiesDomain.InternalMessage ?? string.Empty);
    }

    public partial DetailTotalCategoryDto MapToDto(DetailTotalCategoryDomain src);
}