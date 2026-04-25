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
            .NotEmpty().WithMessage(ExpenseManagementResources.CategoryTypeViewModelNameRequired)
            .WithError(ErrorCode.NameRequired, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.CategoryTypeViewModelNameRequired))

            .Length(1, CategoryTypeDomain.MaxNameLength)
            .WithMessage(string.Format(ExpenseManagementResources.CategoryTypeViewModelNameTooLong, CategoryTypeDomain.MaxNameLength))
            .WithError(ErrorCode.NameTooLong, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.CategoryTypeViewModelNameTooLong), CategoryTypeDomain.MaxNameLength)

            .MustAsync(async (name, cancellation) => await expensePresentationValidationService.IsCategoryTypeNameAvailableAsync(name, cancellation))
            .WithMessage(ExpenseManagementResources.CategoryTypeViewModelNameAlreadyExists)
            .WithError(ErrorCode.NameAlreadyExists, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.CategoryTypeViewModelNameAlreadyExists));

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage(ExpenseManagementResources.CategoryTypeViewModelColorRequired)
            .WithError(ErrorCode.ColorRequired, ExpenseManagementResources.ResourceManager, nameof(ExpenseManagementResources.CategoryTypeViewModelColorRequired));
    }
}