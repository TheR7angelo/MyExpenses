using System.ComponentModel.DataAnnotations;
using MyExpenses.Application.Dtos.Accounts;

namespace MyExpenses.Application.Interfaces.IServices;

public interface IAccountValidationService
{
    public Task<ValidationResult> IsAccountValid(AccountDto accountDto, CancellationToken cancellationToken = default);
}