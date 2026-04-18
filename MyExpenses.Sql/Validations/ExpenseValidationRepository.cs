using Microsoft.EntityFrameworkCore;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Sql.Context;

namespace MyExpenses.Sql.Validations;

public class ExpenseValidationRepository(IDbContextFactory<DataBaseContext> dbContextFactory) : IExpenseValidationRepository
{
    public async Task<bool> IsCategoryTypeNameAlreadyExistAsync(string categoryTypeName, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.TCategoryTypes.AnyAsync(a => a.Name == categoryTypeName, cancellationToken: cancellationToken);
    }
}