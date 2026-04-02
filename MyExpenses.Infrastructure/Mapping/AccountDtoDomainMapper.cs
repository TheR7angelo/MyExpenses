using Domain.Models.Accounts;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Interfaces.Mappings;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Infrastructure.Mapping;

[Mapper]
public partial class AccountDtoDomainMapper : IAccountDtoDomainMapper
{
    public partial TotalByAccountDto MapToDto(TotalByAccountDomain src);

    public partial AccountDto MapToDto(AccountDomain src);

    public partial AccountDomain MapToDomain(AccountDto src);

    public partial CurrencyDto MapToDto(CurrencyDomain src);

    public partial AccountTypeDto MapToDto(AccountTypeDomain src);

    public partial AccountTypeDomain MapToDomain(AccountTypeDto src);
}
