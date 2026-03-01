using Domain.Models.Accounts;
using MyExpenses.Application.Models.Accounts;

namespace MyExpenses.Application.Interfaces;

public interface IAccountServices
{
    public Task<IEnumerable<TotalByAccountDto>> GetAllTotalByAccountAsync();
}