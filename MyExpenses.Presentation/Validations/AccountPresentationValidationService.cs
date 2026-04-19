using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Validations;

public class AccountPresentationValidationService(IAccountValidationRepository accountValidationRepository,
    ILogger<AccountPresentationValidationService> logger)
    : IAccountPresentationValidationService
{
    public async Task<bool> IsAccountTypeNameAvailableAsync(string input, AccountTypeViewModel accountTypeViewModel,
        CancellationToken cancellationToken = default)
    {
        using var scope = logger.BeginScope("Checking account type name availability. AccountTypeId={AccountTypeId}, Input={Input}",
            accountTypeViewModel.Id, input);

        logger.LogInformation("Starting validation for account type name availability");

        try
        {
            var alreadyExists = await accountValidationRepository.IsAccountTypeNameAlreadyExistAsync(
                input,
                accountTypeViewModel.Id,
                cancellationToken);

            if (alreadyExists)
            {
                logger.LogInformation("Account type name is already used");
                return false;
            }

            logger.LogInformation("Account type name is available");
            return true;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Validation was canceled");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while checking account type name availability");
            throw;
        }
    }

    public async Task<bool> IsAccountTypeNameAvailableAsync(string input, CancellationToken cancellationToken = default)
    {
        using var scope = logger.BeginScope("Checking account type name availability. Input={Input}", input);

        logger.LogInformation("Starting validation for account type name availability");

        try
        {
            var alreadyExists = await accountValidationRepository.IsAccountTypeNameAlreadyExistAsync(
                input, cancellationToken);

            if (alreadyExists)
            {
                logger.LogInformation("Account type name is already used");
                return false;
            }

            logger.LogInformation("Account type name is available");
            return true;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Validation was canceled");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while checking account type name availability");
            throw;
        }
    }
}