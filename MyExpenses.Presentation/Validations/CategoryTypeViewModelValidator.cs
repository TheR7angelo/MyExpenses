using Domain.Models.Categories;
using Domain.Models.Validation;
using FluentValidation;
using MyExpenses.Presentation.Resources.Resx.ExpenseManagementResources;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Validations;

public class CategoryTypeViewModelValidator : AbstractValidator<CategoryTypeViewModel>
{
    public CategoryTypeViewModelValidator(IExpensePresentationValidationService expensePresentationValidationService)
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required")
            .WithError(ErrorCode.NameRequired, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.CategoryTypeViewModelNameRequired))

            .Length(1, CategoryTypeDomain.MaxNameLength)
            .WithMessage($"Name must be between 1 and {CategoryTypeDomain.MaxNameLength} characters")
            .WithError(ErrorCode.NameTooLong, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.CategoryTypeViewModelNameTooLong), CategoryTypeDomain.MaxNameLength)

            .MustAsync(async (name, cancellation) => !await expensePresentationValidationService.IsCategoryTypeNameAvailableAsync(name, cancellation))
            .WithMessage("Category type name must be unique")
            .WithError(ErrorCode.NameAlreadyExists, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.CategoryTypeViewModelNameAlreadyExists));

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Color is required")
            .WithError(ErrorCode.ColorRequired, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.CategoryTypeViewModelColorRequired));
    }
}