using Domain.Models.Validation;
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

    public partial void Merge(TotalByAccountViewModel src, TotalByAccountViewModel dest);

    [MapProperty(nameof(AccountDto.AccountTypeDto), nameof(AccountViewModel.AccountTypeViewModel))]
    [MapProperty(nameof(AccountDto.CurrencyDto), nameof(AccountViewModel.CurrencyViewModel))]
    public partial AccountViewModel MapToViewModel(AccountDto src);

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

    public partial CurrencyViewModel Clone(CurrencyViewModel src);

    public partial void Merge(AccountViewModel src, AccountViewModel dest);

    [MapperIgnoreTarget(nameof(TotalByAccountViewModel.IsDeleting))]
    [MapperIgnoreTarget(nameof(TotalByAccountViewModel.Total))]
    [MapperIgnoreTarget(nameof(TotalByAccountViewModel.TotalPointed))]
    [MapperIgnoreTarget(nameof(TotalByAccountViewModel.TotalNotPointed))]
    [MapperIgnoreSource(nameof(AccountViewModel.HasErrors))]
    [MapperIgnoreSource(nameof(AccountViewModel.AccountTypeViewModel))]
    [MapperIgnoreSource(nameof(AccountViewModel.Active))]
    [MapperIgnoreSource(nameof(AccountViewModel.DateAdded))]
    [MapProperty(nameof(AccountViewModel.Id), nameof(TotalByAccountViewModel.Id))]
    [MapProperty(nameof(AccountViewModel.Name), nameof(TotalByAccountViewModel.Name))]
    [MapProperty(nameof(AccountViewModel.CurrencyViewModel.Symbol), nameof(TotalByAccountViewModel.Symbol))]
    public partial void Merge(AccountViewModel src, TotalByAccountViewModel dest);

    public Result<AccountTypeViewModel> MapToViewModel(Result<AccountTypeDto> src)
        => src.Map(MapToViewModel);

    public Result<CurrencyViewModel> MapToViewModel(Result<CurrencyDto> src)
        => src.Map(MapToViewModel);

    public Result<AccountViewModel> MapToViewModel(Result<AccountDto> src)
        => src.Map(MapToViewModel);
}
