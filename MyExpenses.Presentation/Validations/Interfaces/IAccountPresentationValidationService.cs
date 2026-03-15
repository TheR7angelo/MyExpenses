using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Validations.Interfaces;

public interface IAccountPresentationValidationService
{
    public Task<bool> IsAccountValid(AccountViewModel accountViewModel, CancellationToken cancellationToken = default);

    public Task<bool> IsAccountNameAlreadyExist(string accountName, CancellationToken cancellationToken = default);
}