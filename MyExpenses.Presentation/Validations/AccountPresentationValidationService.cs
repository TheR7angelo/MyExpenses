using Domain.Interfaces;
using Domain.Models.Validation;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.Mappings;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Validations;

public class AccountPresentationValidationService(IAccountDtoViewModelMapper mapper, IAccountDtoDomainMapper domainMapper,
    IAccountDomainValidationService accountDomainValidationService, IAccountValidationRepository accountValidationRepository,
    ILogger<AccountPresentationValidationService> logger)
    : IAccountPresentationValidationService
{
    // public async Task<bool> IsAccountValid(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    // {
    //     if (!accountViewModel.IsDirty) return true;
    //
    //     if (accountViewModel.HasErrors) return false;
    //
    //     return true;
    //
    //     // if (!accountViewModel.IsNameDirty) return true;
    //     //
    //     // var dto = mapper.MapToDto(accountViewModel);
    //     //
    //     //
    //     // var domain = domainMapper.MapToDomain(dto);
    //     //
    //     // var isFormatValid = await accountDomainValidationService.IsAccountNameValid(domain, cancellationToken);
    //     // if (!isFormatValid) return false;
    //     //
    //     // var alreadyExists = await accountValidationRepository.IsAccountNameAlreadyExistAsync(dto, cancellationToken);
    //     //
    //     // return !alreadyExists;
    // }
    // public Task<Result> IsAccountValid(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async Task<Result> IsAccountTypeValid(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default)
    // {
    //     if (!accountTypeViewModel.IsDirty) return await Task.FromResult(Result.Success());
    //
    //     // TODO continue
    //     if (accountTypeViewModel.HasErrors)
    //     {
    //         var domainValidationResult = accountTypeViewModel.GetErrorCodes();
    //         var errors = domainValidationResult.Select(e => new { e.ErrorCode, e.InternalMessage });
    //         logger.LogError("Validation failed with errors: {@Errors}", errors);
    //     }
    //
    //     return await Task.FromResult(Result.Success());
    // }

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