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
    public Task<Result> IsAccountValid(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> IsAccountTypeValid(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default)
    {
        if (!accountTypeViewModel.IsDirty) return await Task.FromResult(Result.Success());

        if (accountTypeViewModel.HasErrors)
        {
            var domainValidationResult = accountTypeViewModel.GetErrorCodes();
            var errors = domainValidationResult.Select(e => e.ErrorMessage);
            logger.LogError("Validation failed with errors: {@Errors}", errors);
        }

        return await Task.FromResult(Result.Success());
    }
}