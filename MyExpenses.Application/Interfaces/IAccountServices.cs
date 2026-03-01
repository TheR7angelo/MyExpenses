using MyExpenses.Application.Dtos.Accounts;

namespace MyExpenses.Application.Interfaces;

public interface IAccountServices
{
    public Task<IEnumerable<TotalByAccountDto>> GetAllTotalByAccountAsync();
}