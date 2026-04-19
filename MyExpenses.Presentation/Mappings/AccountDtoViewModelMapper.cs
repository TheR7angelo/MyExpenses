using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Presentation.Mappings;

[Mapper(UseDeepCloning = true)]
public partial class AccountDtoViewModelMapper : IAccountDtoViewModelMapper
{
    [MapperIgnoreTarget(nameof(TotalByAccountViewModel.IsDeleting))]
    public partial TotalByAccountViewModel MapToViewModel(TotalByAccountDto src);

    [MapperIgnoreTarget(nameof(AccountViewModel.IsEditing))]
    [MapProperty(nameof(AccountDto.AccountTypeDto), nameof(AccountViewModel.AccountTypeViewModel))]
    [MapProperty(nameof(AccountDto.CurrencyDto), nameof(AccountViewModel.CurrencyViewModel))]
    public partial AccountViewModel MapToViewModel(AccountDto src);

    [MapperIgnoreSource(nameof(AccountViewModel.IsEditing))]
    [MapperIgnoreSource(nameof(AccountViewModel.HasErrors))]
    [MapProperty(nameof(AccountViewModel.AccountTypeViewModel), nameof(AccountDto.AccountTypeDto))]
    [MapProperty(nameof(AccountViewModel.CurrencyViewModel), nameof(AccountDto.CurrencyDto))]
    public partial AccountDto MapToDto(AccountViewModel src);

    [MapperIgnoreSource(nameof(AccountTypeViewModel.HasErrors))]
    public partial AccountTypeDto MapToDto(AccountTypeViewModel src);

    public partial CurrencyViewModel MapToViewModel(CurrencyDto src);

    [MapperIgnoreSource(nameof(CurrencyViewModel.HasErrors))]
    public partial CurrencyDto MapToDto(CurrencyViewModel src);

    public partial AccountTypeViewModel MapToViewModel(AccountTypeDto src);

    public partial AccountTypeViewModel Clone(AccountTypeViewModel src);
}
