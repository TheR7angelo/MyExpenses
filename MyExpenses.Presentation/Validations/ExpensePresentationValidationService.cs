using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Validations;

public class ExpensePresentationValidationService(ILogger<ExpensePresentationValidationService> logger,
    IExpenseValidationRepository expenseValidationRepository) : IExpensePresentationValidationService
{
    public async Task<bool> IsCategoryTypeNameAvailableAsync(string input, CancellationToken cancellationToken = default)
    {
        using var scope = logger.BeginScope("Checking category type name availability. Input={Input}", input);

        logger.LogInformation("Starting validation for category type name availability");

        try
        {
            var alreadyExists = await expenseValidationRepository.IsCategoryTypeNameAlreadyExistAsync(
                input, cancellationToken);

            if (alreadyExists)
            {
                logger.LogInformation("Category type name is already used");
                return false;
            }

            logger.LogInformation("Category type name is available");
            return true;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Validation was canceled");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while checking category type name availability");
            throw;
        }
    }

    public async Task<bool> IsCategoryTypeNameAvailableAsync(string input, CategoryTypeViewModel categoryTypeViewModel,
        CancellationToken cancellationToken = default)
    {
        using var scope = logger.BeginScope("Checking category type name availability. CategoryTypeId={CategoryTypeId}, Input={Input}",
            categoryTypeViewModel.Id, input);

        logger.LogInformation("Starting validation for account type name availability");

        try
        {
            var alreadyExists = await expenseValidationRepository.IsCategoryTypeNameAlreadyExistAsync(
                input,
                categoryTypeViewModel.Id,
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
}