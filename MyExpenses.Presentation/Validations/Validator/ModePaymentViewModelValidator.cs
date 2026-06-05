using Domain.Models.Expenses;
using Domain.Models.Validation;
using FluentValidation;
using MyExpenses.Presentation.Resources.Resx.ExpenseResources;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Validations.Validator;

public class ModePaymentViewModelValidator : AbstractValidator<ModePaymentViewModel>
{
    public ModePaymentViewModelValidator(IExpensePresentationValidationService expensePresentationValidationService)
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ExpenseResources.ModePaymentViewModelNameRequired)
            .WithError(ErrorCode.NameRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.ModePaymentViewModelNameRequired))

            .Length(1, ModePaymentDomain.MaxNameLength)
            .WithMessage(string.Format(ExpenseResources.ModePaymentViewModelNameTooLong, ModePaymentDomain.MaxNameLength))
            .WithError(ErrorCode.NameTooLong, ExpenseResources.ResourceManager, nameof(ExpenseResources.ModePaymentViewModelNameTooLong), ModePaymentDomain.MaxNameLength)

            .MustAsync(async (name, cancellation) => await expensePresentationValidationService.IsModePayementNameAvailableAsync(name, cancellation))
            .WithMessage(ExpenseResources.ModePaymentViewModelNameAlreadyExists)
            .WithError(ErrorCode.NameAlreadyExists, ExpenseResources.ResourceManager, nameof(ExpenseResources.ModePaymentViewModelNameAlreadyExists))

            .When(x => x.IsNameDirty || x.Id is 0);
    }
}