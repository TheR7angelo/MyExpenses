using Domain.Models.Expenses;
using Domain.Models.Validation;
using FluentValidation;
using MyExpenses.Presentation.Resources.Resx.ExpenseResources;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Validations.Validator;

public class CategoryTypeViewModelValidator : AbstractValidator<CategoryTypeViewModel>
{
    public CategoryTypeViewModelValidator(IExpensePresentationValidationService expensePresentationValidationService)
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ExpenseResources.CategoryTypeViewModelNameRequired)
            .WithError(ErrorCode.NameRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.CategoryTypeViewModelNameRequired))

            .Length(1, CategoryTypeDomain.MaxNameLength)
            .WithMessage(string.Format(ExpenseResources.CategoryTypeViewModelNameTooLong, CategoryTypeDomain.MaxNameLength))
            .WithError(ErrorCode.NameTooLong, ExpenseResources.ResourceManager, nameof(ExpenseResources.CategoryTypeViewModelNameTooLong), CategoryTypeDomain.MaxNameLength)

            .MustAsync(async (name, cancellation) => await expensePresentationValidationService.IsCategoryTypeNameAvailableAsync(name, cancellation))
            .WithMessage(ExpenseResources.CategoryTypeViewModelNameAlreadyExists)
            .WithError(ErrorCode.NameAlreadyExists, ExpenseResources.ResourceManager, nameof(ExpenseResources.CategoryTypeViewModelNameAlreadyExists))

            .When(x => x.IsNameDirty || x.Id is 0);

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage(ExpenseResources.CategoryTypeViewModelColorRequired)
            .WithError(ErrorCode.ColorRequired, ExpenseResources.ResourceManager, nameof(ExpenseResources.CategoryTypeViewModelColorRequired))

            .When(x => x.IsColorDirty || x.Id is 0);
    }
}