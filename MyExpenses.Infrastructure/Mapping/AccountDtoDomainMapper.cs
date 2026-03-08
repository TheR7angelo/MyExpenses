using Domain.Models.Accounts;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Mappings.Interfaces;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Infrastructure.Mapping;

[Mapper]
public partial class AccountDtoDomainMapper : IAccountDtoDomainMapper
{
    public partial TotalByAccountDto MapToDto(TotalByAccountDomain src);
    public partial TotalByAccountDomain MapToDomain(TotalByAccountDto src);
}