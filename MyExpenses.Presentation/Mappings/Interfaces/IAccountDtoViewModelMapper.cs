using Domain.Models.Systems;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Dtos.Categories;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Categories;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Mappings.Interfaces;

public interface IAccountDtoViewModelMapper
{
    public TotalByAccountViewModel MapToViewModel(TotalByAccountDto src);

    public AccountViewModel MapToViewModel(AccountDto src);
    public AccountDto MapToDto(AccountViewModel src);

    public CurrencyViewModel MapToViewModel(CurrencyDto src);

    public AccountTypeViewModel MapToViewModel(AccountTypeDto src);

    public AccountTypeDto MapToDto(AccountTypeViewModel src);

    public CategoryTypeDto MapToDto(CategoryTypeViewModel src);

    //TODO need to move
    public ColorDto MapToDto(ColorDomain src);

    //TODO need to move
    public ColorViewModel MapToViewModel(ColorDto src);
}
