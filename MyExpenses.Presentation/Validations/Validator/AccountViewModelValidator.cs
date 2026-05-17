using Domain.Models.Accounts;
using Domain.Models.Validation;
using FluentValidation;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Validations.Validator;

public class AccountViewModelValidator : AbstractValidator<AccountViewModel>
{
    public AccountViewModelValidator(IAccountValidationRepository repository)
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(AccountResources.AccountViewModelValidatorNameRequired)
            .WithError(ErrorCode.NameRequired, AccountResources.ResourceManager, nameof(AccountResources.AccountViewModelValidatorNameRequired))

            .Length(1, AccountDomain.MaxNameLength).WithMessage(string.Format(AccountResources.AccountViewModelValidatorNameTooLong, AccountDomain.MaxNameLength))
            .WithError(ErrorCode.NameTooLong, AccountResources.ResourceManager, nameof(AccountResources.AccountViewModelValidatorNameTooLong), AccountDomain.MaxNameLength)

            .MustAsync(async (name, cancellation) => !await repository.IsAccountNameAlreadyExistAsync(name, cancellation))

            .When(x => x.IsNameDirty || x.Id is 0);

        RuleFor(x => x.AccountTypeViewModel)
            .NotNull().WithMessage(AccountResources.AccountViewModelValidatorAccountTypeRequired)
            .WithError(ErrorCode.AccountTypeRequired, AccountResources.ResourceManager, nameof(AccountResources.AccountViewModelValidatorAccountTypeRequired))

            .When(x => x.IsAccountTypeViewModelDirty || x.Id is 0);

        RuleFor(x => x.CurrencyViewModel)
            .NotNull().WithMessage(AccountResources.AccountViewModelValidatorCurrencyRequired)
            .WithError(ErrorCode.CurrencyRequired, AccountResources.ResourceManager, nameof(AccountResources.AccountViewModelValidatorCurrencyRequired))

            .When(x => x.IsCurrencyViewModelDirty || x.Id is 0);
    }
}