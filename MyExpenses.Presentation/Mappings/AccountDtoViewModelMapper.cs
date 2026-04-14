using Domain.Models.Systems;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Dtos.Categories;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Categories;
using MyExpenses.Presentation.ViewModels.Systems;
using Riok.Mapperly.Abstractions;

namespace MyExpenses.Presentation.Mappings;

[Mapper]
public partial class AccountDtoViewModelMapper : IAccountDtoViewModelMapper
{
    public partial TotalByAccountViewModel MapToViewModel(TotalByAccountDto src);

    [MapperIgnoreTarget(nameof(AccountViewModel.IsEditing))]
    public partial AccountViewModel MapToViewModel(AccountDto src);

    [MapperIgnoreSource(nameof(AccountViewModel.IsEditing))]
    [MapperIgnoreSource(nameof(AccountViewModel.HasErrors))]
    public partial AccountDto MapToDto(AccountViewModel src);

    [MapperIgnoreSource(nameof(AccountTypeViewModel.HasErrors))]
    public partial AccountTypeDto MapToDto(AccountTypeViewModel src);

    [MapperIgnoreSource(nameof(CategoryTypeViewModel.HasErrors))]
    public partial CategoryTypeDto MapToDto(CategoryTypeViewModel src);

    // TODO need to move
    public partial ColorDto MapToDto(ColorDomain src);

    // TODO need to move
    public partial ColorViewModel MapToViewModel(ColorDto src);

    public partial CurrencyViewModel MapToViewModel(CurrencyDto src);

    public partial AccountTypeViewModel MapToViewModel(AccountTypeDto src);
}
