using Domain.Interfaces;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.Mappings;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Validations;

public class AccountPresentationValidationService(IAccountDtoViewModelMapper mapper, IAccountDtoDomainMapper domainMapper,
    IAccountValidationService accountValidationService, IAccountValidationRepository accountValidationRepository)
    : IAccountPresentationValidationService
{
    public async Task<bool> IsAccountValid(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        if (!accountViewModel.HasNameChanged) return true;

        var dto = mapper.MapToDto(accountViewModel);
        // TODO correct needed ( check all properties before check name !!! )
        var domain = domainMapper.MapToDomain(dto);

        var isFormatValid = await accountValidationService.IsAccountNameValid(domain, cancellationToken);
        if (!isFormatValid) return false;

        var alreadyExists = await accountValidationRepository.IsAccountNameAlreadyExistAsync(dto, cancellationToken);

        return !alreadyExists;
    }
}