using Microsoft.EntityFrameworkCore;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Sql.Context;

namespace MyExpenses.Sql.Validations;

public class AccountValidationRepository(IDbContextFactory<DataBaseContext> dbContextFactory) : IAccountValidationRepository
{
    public async Task<bool> IsAccountNameAlreadyExistAsync(string accountName, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.TAccounts.AnyAsync(a => a.Name == accountName, cancellationToken);
    }

    public async Task<bool> IsAccountTypeNameAlreadyExistAsync(string accountTypeName, int id, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.TAccountTypes.AnyAsync(a => a.Name == accountTypeName && a.Id != id, cancellationToken);
    }

    public async Task<bool> IsAccountTypeNameAlreadyExistAsync(string accountTypeName, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.TAccountTypes.AnyAsync(a => a.Name == accountTypeName, cancellationToken: cancellationToken);
    }

    public async Task<bool> IsCategoryTypeNameAlreadyExistAsync(string categoryTypeName, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.TCategoryTypes.AnyAsync(a => a.Name == categoryTypeName, cancellationToken: cancellationToken);
    }
}