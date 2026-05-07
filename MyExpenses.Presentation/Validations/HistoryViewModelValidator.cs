using Domain.Models.Expenses;
using Domain.Models.Validation;
using FluentValidation;
using MyExpenses.Presentation.Resources.Resx.ExpenseResources;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Validations;

public class HistoryViewModelValidator : AbstractValidator<HistoryViewModel>
{
    public HistoryViewModelValidator()
    {
        RuleFor(x => x.AccountViewModel)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ExpenseResources.HistoryViewModelValidatorAccountViewModelRequired)
            .WithError(ErrorCode.AccountRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.HistoryViewModelValidatorAccountViewModelRequired))
            ;
            // .When(x => x.IsAccountViewModelDirty);

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Length(1, HistoryDomain.MaxDescriptionLength).WithMessage(ExpenseResources.HistoryViewModelValidatorDescriptionLength)
            .WithError(ErrorCode.DescriptionTooLong, ExpenseResources.ResourceManager, nameof(ExpenseResources.HistoryViewModelValidatorDescriptionLength))
            ;
            // .When(x => x.IsDescriptionDirty);

        RuleFor(x => x.CategoryTypeViewModel)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ExpenseResources.HistoryViewModelValidatorCategoryTypeViewModelRequired)
            .WithError(ErrorCode.CategoryTypeRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.HistoryViewModelValidatorCategoryTypeViewModelRequired))
            ;
            // .When(x => x.IsCategoryTypeViewModelDirty);

        RuleFor(x => x.ModePaymentViewModel)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ExpenseResources.HistoryViewModelValidatorModePaymentViewModelRequired)
            .WithError(ErrorCode.ModePaymentRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.HistoryViewModelValidatorModePaymentViewModelRequired))
            ;
            // .When(x => x.IsModePaymentViewModelDirty);

        RuleFor(x => x.Value)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ExpenseResources.HistoryViewModelValidatorValueRequired)
            .WithError(ErrorCode.ValueRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.HistoryViewModelValidatorValueRequired))
            ;
            // .When(x => x.IsValueDirty);

        RuleFor(x => x.Date)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ExpenseResources.HistoryViewModelValidatorDateRequired)
            .WithError(ErrorCode.DateRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.HistoryViewModelValidatorDateRequired))
            ;
            // .When(x => x.IsDateDirty);

            RuleFor(x => x.PlaceViewModel)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(ExpenseResources.HistoryViewModelValidatorPlaceViewModelRequired)
                .WithError(ErrorCode.PlaceRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.HistoryViewModelValidatorPlaceViewModelRequired))
                ;
            // .When(x => x.IsPlaceViewModelDirty);
    }
}