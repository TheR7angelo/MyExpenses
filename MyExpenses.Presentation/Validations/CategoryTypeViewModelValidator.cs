using FluentValidation;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Validations;

public class CategoryTypeViewModelValidator : AbstractValidator<CategoryTypeViewModel>
{
    public CategoryTypeViewModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
    }
}