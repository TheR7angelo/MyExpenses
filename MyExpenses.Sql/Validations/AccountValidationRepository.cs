using Microsoft.EntityFrameworkCore;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Sql.Context;

namespace MyExpenses.Sql.Validations;

public class AccountValidationRepository(IDbContextFactory<DataBaseContext> dbContextFactory) : IAccountValidationRepository
{
    public async Task<bool> IsAccountNameAlreadyExistAsync(AccountDto accountDto, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.TAccounts.AnyAsync(a => a.Name == accountDto.Name && a.Id != accountDto.Id, cancellationToken);
    }

    public async Task<bool> IsAccountTypeNameAlreadyExistAsync(string accountTypeName, int id, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.TAccountTypes.AnyAsync(a => a.Name == accountTypeName && a.Id != id, cancellationToken);
    }
}