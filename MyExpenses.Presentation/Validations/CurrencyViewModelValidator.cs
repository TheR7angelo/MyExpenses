using Domain.Models.Accounts;
using Domain.Models.Expenses;
using Domain.Models.Validation;
using FluentValidation;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Resources.Resx.ExpenseManagementResources;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Validations;

public class CurrencyViewModelValidator : AbstractValidator<CurrencyViewModel>
{
    public CurrencyViewModelValidator(IAccountPresentationValidationService expensePresentationValidationService)
    {
        RuleFor(x => x.Symbol)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(AccountResources.CurrencyViewModelNameRequired)
            .WithError(ErrorCode.NameRequired, AccountResources.ResourceManager, nameof(AccountResources.CurrencyViewModelNameRequired))

            .Length(1, CurrencyDomain.MaxSymbolLength)
            .WithMessage(string.Format(AccountResources.CurrencyViewModelNameTooLong, CategoryTypeDomain.MaxNameLength))
            .WithError(ErrorCode.NameTooLong, ExpenseManagementResources.ResourceManager, nameof(AccountResources.CurrencyViewModelNameTooLong), CurrencyDomain.MaxSymbolLength)

            .MustAsync(async (name, cancellation) => await expensePresentationValidationService.IsCurrencySymbolIsAvailableAsync(name, cancellation))
            .WithMessage(AccountResources.CurrencyViewModelNameAlreadyExists)
            .WithError(ErrorCode.NameAlreadyExists, ExpenseManagementResources.ResourceManager, nameof(AccountResources.CurrencyViewModelNameAlreadyExists))

            .When(x => x.IsSymbolDirty);
    }
}