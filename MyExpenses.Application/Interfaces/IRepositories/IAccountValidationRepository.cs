namespace MyExpenses.Application.Interfaces.IRepositories;

public interface IAccountValidationRepository
{
    public Task<bool> IsAccountNameAlreadyExist(string name, CancellationToken cancellationToken = default);
}