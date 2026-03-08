using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Mappings.Interfaces;
using MyExpenses.Application.ViewModels.Accounts;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Application.Mappings;

[Mapper]
public partial class AccountDtoViewModelMapper : IAccountDtoViewModelMapper
{
    public partial TotalByAccountViewModel MapToViewModel(TotalByAccountDto src);

    public partial TotalByAccountDto MapToDomain(TotalByAccountViewModel src);
}
