using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Validations;

public class AccountPresentationValidationService(IAccountValidationRepository accountValidationRepository) : IAccountPresentationValidationService
{
    public async Task<bool> IsAccountValid(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        if (accountViewModel.HasNameChanged)
        {
            return await IsAccountNameAlreadyExist(accountViewModel.Name, cancellationToken);
        }

        return false;
    }

    public async Task<bool> IsAccountNameAlreadyExist(string accountName, CancellationToken cancellationToken = default)
    {
        return await accountValidationRepository.IsAccountNameAlreadyExist(accountName, cancellationToken);
    }
}