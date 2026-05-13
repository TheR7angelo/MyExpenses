using Domain.Models.Expenses;
using Domain.Models.Validation;
using FluentValidation;
using MyExpenses.Presentation.Resources.Resx.ExpenseResources;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Validations;

public class BankTransferViewModelValidator : AbstractValidator<BankTransferViewModel>
{
    public BankTransferViewModelValidator()
    {
        RuleFor(x => x.Value)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ExpenseResources.BankTransferViewModelValidatorValueRequired)
            .WithError(ErrorCode.ValueRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.BankTransferViewModelValidatorValueRequired))

            ;
            // .When(x => x.IsValueDirty);

        RuleFor(x => x.FromAccount)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ExpenseResources.BankTransferViewModelValidatorFromAccountRequired)
            .WithError(ErrorCode.AccountRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.BankTransferViewModelValidatorFromAccountRequired))

            ;
            // .When(x => x.IsFromAccountDirty);

        RuleFor(x => x.ToAccount)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ExpenseResources.BankTransferViewModelValidatorToAccountRequired)
            .WithError(ErrorCode.AccountRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.BankTransferViewModelValidatorToAccountRequired))

            ;
            // .When(x => x.IsToAccountDirty);

        RuleFor(x => x.MainReason)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ExpenseResources.BankTransferViewModelValidatorMainReasonRequired)
            .WithError(ErrorCode.DescriptionRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.BankTransferViewModelValidatorMainReasonRequired))

            .Length(1, BankTransferDomain.MaxMainReasonLength).WithMessage(string.Format(ExpenseResources.BankTransferViewModelValidatorMainReasonTooLong, BankTransferDomain.MaxMainReasonLength))
            .WithError(ErrorCode.DescriptionTooLong, ExpenseResources.ResourceManager, nameof(ExpenseResources.BankTransferViewModelValidatorMainReasonTooLong), BankTransferDomain.MaxMainReasonLength)

            ;
            // .When(x => x.IsMainReasonDirty);

        RuleFor(x => x.AdditionalReason)
            .MaximumLength(BankTransferDomain.MaxAdditionalReasonLength).WithMessage(string.Format(ExpenseResources.BankTransferViewModelValidatorAdditionalReasonTooLong, BankTransferDomain.MaxAdditionalReasonLength))
            .WithError(ErrorCode.DescriptionTooLong, ExpenseResources.ResourceManager, nameof(ExpenseResources.BankTransferViewModelValidatorAdditionalReasonTooLong), BankTransferDomain.MaxAdditionalReasonLength)

            ;
            // .When(x => x.IsAdditionalReasonDirty);

        RuleFor(x => x.Date)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ExpenseResources.BankTransferViewModelValidatorDateRequired)
            .WithError(ErrorCode.DateRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.BankTransferViewModelValidatorDateRequired), ExpenseResources.BankTransferViewModelValidatorDateRequired)

            ;
            // .When(x => x.IsDateDirty);
    }
}