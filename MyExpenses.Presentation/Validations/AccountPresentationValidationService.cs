using Domain.Interfaces;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.Mappings;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Validations;

public class AccountPresentationValidationService(IAccountDtoViewModelMapper mapper, IAccountDtoDomainMapper domainMapper,
    IAccountDomainValidationService accountDomainValidationService, IAccountValidationRepository accountValidationRepository)
    : IAccountPresentationValidationService
{
    public async Task<bool> IsAccountValid(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        if (!accountViewModel.IsNameDirty) return true;

        var dto = mapper.MapToDto(accountViewModel);


        var domain = domainMapper.MapToDomain(dto);

        var isFormatValid = await accountDomainValidationService.IsAccountNameValid(domain, cancellationToken);
        if (!isFormatValid) return false;

        var alreadyExists = await accountValidationRepository.IsAccountNameAlreadyExistAsync(dto, cancellationToken);

        return !alreadyExists;
    }
}