using Domain.Models.Accounts;
using Domain.Models.Categories;
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

    public partial AccountTypeDto MapToDto(AccountTypeDomain src);

    public partial AccountTypeDomain MapToDomain(AccountTypeDto src);

    public partial CategoryTypeDomain MapToDomain(CategoryTypeDto src);
}
