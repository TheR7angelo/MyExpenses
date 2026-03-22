using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Validations;

public class AccountPresentationValidationService(IAccountDtoViewModelMapper mapper, IAccountValidationRepository accountValidationRepository)
    : IAccountPresentationValidationService
{
    public Task<bool> IsAccountValid(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {

    }
}