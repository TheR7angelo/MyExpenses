using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Presentation.Mappings;

[Mapper]
public partial class AccountDtoViewModelMapper : IAccountDtoViewModelMapper
{
    public partial TotalByAccountViewModel MapToViewModel(TotalByAccountDto src);

    [MapperIgnoreTarget(nameof(AccountViewModel.IsEditing))]
    public partial AccountViewModel MapToViewModel(AccountDto src);

    [MapperIgnoreSource(nameof(AccountViewModel.IsEditing))]
    public partial AccountDto MapToDto(AccountViewModel src);

    public partial CurrencyViewModel MapToViewModel(CurrencyDto src);

    public partial AccountTypeViewModel MapToViewModel(AccountTypeDto src);
}
