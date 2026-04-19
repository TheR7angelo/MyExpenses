using Domain.Models.Accounts;
using Domain.Models.Validation;
using FluentValidation;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Validations;

public class AccountTypeViewModelValidator : AbstractValidator<AccountTypeViewModel>
{
    public AccountTypeViewModelValidator(IAccountValidationRepository repository)
    {
        RuleFor(s => s.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(AccountResources.AccountTypeViewModelValidatorNameRequired)
            .WithError(ErrorCode.NameRequired, AccountResources.ResourceManager, nameof(AccountResources.AccountTypeViewModelValidatorNameRequired))

            .Length(1, AccountTypeDomain.MaxNameLength).WithMessage(string.Format(AccountResources.AccountTypeViewModelValidatorNameTooLong, AccountTypeDomain.MaxNameLength))
            .WithError(ErrorCode.NameTooLong, AccountResources.ResourceManager, nameof(AccountResources.AccountTypeViewModelValidatorNameTooLong), AccountTypeDomain.MaxNameLength)

            .MustAsync(async (name, cancellation) => !await repository.IsAccountTypeNameAlreadyExistAsync(name, cancellation))
            .WithMessage(AccountResources.AccountTypeViewModelValidatorNameAlreadyExists)
            .WithError(ErrorCode.NameAlreadyExists, AccountResources.ResourceManager, nameof(AccountResources.AccountTypeViewModelValidatorNameAlreadyExists));
    }
}