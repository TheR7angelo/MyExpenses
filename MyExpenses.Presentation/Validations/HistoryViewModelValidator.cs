using Domain.Models.Expenses;
using Domain.Models.Validation;
using FluentValidation;
using MyExpenses.Presentation.Resources.Resx.ExpenseManagementResources;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Validations;

public class HistoryViewModelValidator : AbstractValidator<HistoryViewModel>
{
    public HistoryViewModelValidator()
    {
        RuleFor(x => x.AccountViewModel)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ExpenseManagementResources.HistoryViewModelValidatorAccountViewModelRequired)
            .WithError(ErrorCode.AccountRequired, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.HistoryViewModelValidatorAccountViewModelRequired))
            ;
            // .When(x => x.IsAccountViewModelDirty);

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Length(1, HistoryDomain.MaxDescriptionLength).WithMessage(ExpenseManagementResources.HistoryViewModelValidatorDescriptionLength)
            .WithError(ErrorCode.DescriptionTooLong, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.HistoryViewModelValidatorDescriptionLength))
            ;
            // .When(x => x.IsDescriptionDirty);

        RuleFor(x => x.CategoryTypeViewModel)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ExpenseManagementResources.HistoryViewModelValidatorCategoryTypeViewModelRequired)
            .WithError(ErrorCode.CategoryTypeRequired, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.HistoryViewModelValidatorCategoryTypeViewModelRequired))
            ;
            // .When(x => x.IsCategoryTypeViewModelDirty);

        RuleFor(x => x.ModePaymentViewModel)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ExpenseManagementResources.HistoryViewModelValidatorModePaymentViewModelRequired)
            .WithError(ErrorCode.ModePaymentRequired, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.HistoryViewModelValidatorModePaymentViewModelRequired))
            ;
            // .When(x => x.IsModePaymentViewModelDirty);

        RuleFor(x => x.Value)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ExpenseManagementResources.HistoryViewModelValidatorValueRequired)
            .WithError(ErrorCode.ValueRequired, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.HistoryViewModelValidatorValueRequired))
            ;
            // .When(x => x.IsValueDirty);

        RuleFor(x => x.Date)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ExpenseManagementResources.HistoryViewModelValidatorDateRequired)
            .WithError(ErrorCode.DateRequired, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.HistoryViewModelValidatorDateRequired))
            ;
            // .When(x => x.IsDateDirty);

            RuleFor(x => x.PlaceViewModel)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(ExpenseManagementResources.HistoryViewModelValidatorPlaceViewModelRequired)
                .WithError(ErrorCode.PlaceRequired, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.HistoryViewModelValidatorPlaceViewModelRequired))
                ;
            // .When(x => x.IsPlaceViewModelDirty);
    }
}