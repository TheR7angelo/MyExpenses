using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Mappings.Interfaces;

public interface IAccountDtoViewModelMapper
{
    public TotalByAccountViewModel MapToViewModel(TotalByAccountDto src);

    public AccountViewModel MapToViewModel(AccountDto src);
    public AccountDto MapToDto(AccountViewModel src);

    public CurrencyViewModel MapToViewModel(CurrencyDto src);

    public AccountTypeViewModel MapToViewModel(AccountTypeDto src);
}
