using Domain.Models.Accounts;
using MyExpenses.Application.Dtos.Accounts;

namespace MyExpenses.Application.Interfaces.Mappings;

public interface IAccountDtoDomainMapper
{
    public TotalByAccountDto MapToDto(TotalByAccountDomain src);

    public AccountDto MapToDto(AccountDomain src);

    public CurrencyDto MapToDto(CurrencyDomain src);

    public AccountTypeDto MapToDto(AccountTypeDomain src);
}