using Domain.Models.Validation;
using FluentValidation;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.SharedUtils.Resources.Resx.AddEditAccount;

namespace MyExpenses.Presentation.Validations;

public class AccountTypeViewModelValidator : AbstractValidator<AccountTypeViewModel>
{
    public AccountTypeViewModelValidator(IAccountValidationRepository repository)
    {
        RuleFor(s => s.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(AddEditAccountResources.ButtonCancelContent)
            .WithError(ErrorCode.NameRequired, AddEditAccountResources.ResourceManager, nameof(AddEditAccountResources.ButtonCancelContent))

            .Length(1, 100).WithMessage("Name must be between 1 and 100 characters long").WithState(_ => ErrorCode.NameTooLong)

            .MustAsync(async (name, cancellation) => !await repository.IsAccountTypeNameAlreadyExistAsync(name, cancellation))
            .WithMessage("Name must be unique").WithState(_ => ErrorCode.NameAlreadyExists);
    }
}