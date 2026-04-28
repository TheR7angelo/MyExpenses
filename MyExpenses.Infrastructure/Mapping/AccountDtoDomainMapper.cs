using Domain.Models.Accounts;
using Domain.Models.Categories;
using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Dtos.Categories;
using MyExpenses.Application.Interfaces.Mappings;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Infrastructure.Mapping;

[Mapper]
public partial class AccountDtoDomainMapper : IAccountDtoDomainMapper
{
    public partial TotalByAccountDto MapToDto(TotalByAccountDomain src);

    [MapProperty(nameof(AccountDomain.AccountTypeDomain), nameof(AccountDto.AccountTypeDto))]
    [MapProperty(nameof(AccountDomain.CurrencyDomain), nameof(AccountDto.CurrencyDto))]
    public partial AccountDto MapToDto(AccountDomain src);

    [MapProperty(nameof(AccountDto.AccountTypeDto), nameof(AccountDomain.AccountTypeDomain))]
    [MapProperty(nameof(AccountDto.CurrencyDto), nameof(AccountDomain.CurrencyDomain))]
    public partial AccountDomain MapToDomain(AccountDto src);

    public partial CurrencyDto MapToDto(CurrencyDomain src);

    public partial CurrencyDomain MapToDomain(CurrencyDto src);

    public partial AccountTypeDto MapToDto(AccountTypeDomain src);

    public partial AccountTypeDomain MapToDomain(AccountTypeDto src);

    public partial CategoryTypeDomain MapToDomain(CategoryTypeDto src);

    public Result<AccountTypeDto> MapToDto(Result<AccountTypeDomain> source)
    {
        if (!source.IsSuccess) return Result<AccountTypeDto>.Failure(source.ErrorCode, source.InternalMessage ?? string.Empty);

        var dto = source.Value is null ? null : MapToDto(source.Value);
        return Result<AccountTypeDto>.Success(dto, source.InternalMessage);
    }

    public Result<CurrencyDto> MapToDto(Result<CurrencyDomain> src)
    {
        if (!src.IsSuccess) return Result<CurrencyDto>.Failure(src.ErrorCode, src.InternalMessage ?? string.Empty);

        var dto = src.Value is null ? null : MapToDto(src.Value);
        return Result<CurrencyDto>.Success(dto, src.InternalMessage);
    }

    public Result<AccountDto> MapToDto(Result<AccountDomain> success)
        => success.Map(MapToDto);
}
