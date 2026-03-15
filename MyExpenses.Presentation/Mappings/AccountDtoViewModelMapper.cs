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
    [MapProperty(nameof(AccountDto.Name), nameof(AccountViewModel.Name))]
    [MapProperty(nameof(AccountDto.Name), nameof(AccountViewModel.OriginalName))]
    public partial AccountViewModel MapToViewModel(AccountDto src);

    public partial CurrencyViewModel MapToViewModel(CurrencyDto src);

    public partial AccountTypeViewModel MapToViewModel(AccountTypeDto src);
}
