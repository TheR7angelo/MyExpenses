using Domain.Models.Dependencies;
using Domain.Models.Systems;
using Domain.Models.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Mappings;

namespace MyExpenses.Sql.Repositories;

public class SystemRepository(IDbContextFactory<DataBaseContext> dbContextFactory,
    IExpenseRepository expenseRepository,
    ILogger<SystemRepository> logger) : ISystemRepository
{
    public async Task<int> GetAllColorCount(CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Counting all colors available");
        var count = await context.TColors.CountAsync(cancellationToken);
        logger.LogInformation("Loaded {Count} colors", count);

        return count;
    }

    public async Task<ColorDomain> GetRandomColor(CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var colorCount = await GetAllColorCount(cancellationToken);
        var randomIndex = Random.Shared.Next(0, colorCount);

        logger.LogInformation("Getting random color with index {Index}", randomIndex);
        var randomColor = await context.TColors
            .AsNoTracking()
            .Skip(randomIndex)
            .ProjectToDomain()
            .FirstAsync(cancellationToken);

        logger.LogInformation("Loaded random color with name {Name}", randomColor.Name);
        return randomColor;
    }

    public async Task<IEnumerable<ColorDomain>> GetAllColors(CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all colors");

        var colors = await context.TColors
            .AsNoTracking()
            .ProjectToDomain()
            .ToListAsync(cancellationToken);
        logger.LogInformation("Loaded {Count} colors", colors.Count);

        return colors;
    }

    public async Task<Result<ColorDomain>> CreateColorAsync(ColorDomain colorDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            var color = colorDomain.MapToEntity();

            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            logger.LogInformation("Adding color with name {ColorName}", color.Name);

            context.TColors.Add(color);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Color with name {ColorName} was successfully added", color.Name);
            return Result<ColorDomain>.Success(colorDomain);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create color with name {ColorName}", colorDomain.Name);
            return Result<ColorDomain>.Failure(ErrorCode.DatabaseError, e.Message);
        }
    }

    public async Task<Result<ColorDomain>> UpdateColorAsync(ColorDomain colorDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            var color = colorDomain.MapToEntity();

            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            logger.LogInformation("Updating color with id {ColorId}", color.Id);

            var entity = await context.TColors.FirstOrDefaultAsync(s => s.Id == color.Id, cancellationToken);
             if (entity is null)
            {
                logger.LogWarning("Color with id {ColorId} not found", color.Id);
                return Result<ColorDomain>.Failure(ErrorCode.NotFound, "Color not found");
            }

            color.Merge(entity);

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Color with id {ColorId} was successfully updated", color.Id);
            return Result<ColorDomain>.Success(colorDomain);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update color with id {ColorId}", colorDomain.Id);
            return Result<ColorDomain>.Failure(ErrorCode.DatabaseError, e.Message);
        }
    }

    public async Task<DeletionResult> DeleteColorAsync(ColorDomain colorDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Deleting color with name {ColorName} and id {ColorId}", colorDomain.Name, colorDomain.Id);

        var colorEntity = await context.TColors.FirstOrDefaultAsync(s => s.Id == colorDomain.Id, cancellationToken: cancellationToken);

        if (colorEntity is null && colorDomain.Id is 0)
        {
            logger.LogWarning("Color with id {ColorId} was not found", colorDomain.Id);
            return DeletionResult.Success("Color was not persisted, so nothing had to be deleted");
        }

        if (colorEntity is null)
        {
            logger.LogWarning("Color with id {ColorId} was not found", colorDomain.Id);
            return DeletionResult.Failure(ErrorCode.NotFound, $"The color with id {colorDomain.Id} was not found");
        }

        try
        {
            var categoryTypeEntities = await expenseRepository.GetAllByColorAsync(colorDomain, cancellationToken);
            if (!categoryTypeEntities.IsSuccess)
            {
                logger.LogError("Failed to retrieve category types associated with color id {ColorId}: {ErrorMessage}", colorDomain.Id, categoryTypeEntities.InternalMessage);
                return DeletionResult.Failure(ErrorCode.DatabaseError, "Failed to retrieve category types associated");
            }

            var enumerable = categoryTypeEntities.Value!.ToArray();

            var categoryTypeIds = enumerable.Select(c => c.Id).ToArray();
            var expenseIdsTask = expenseRepository.GetAllExpenseIdAsync(enumerable, cancellationToken);
            var bankTransferIdsTask = expenseRepository.GetAllBankTransferIdsAsync(enumerable, cancellationToken);
            var recurringExpenseIdsTask = expenseRepository.GetAllRecurringTransactionIdsAsync(enumerable, cancellationToken);

            await Task.WhenAll(expenseIdsTask, bankTransferIdsTask, recurringExpenseIdsTask);

            var expenseIds = expenseIdsTask.Result;
            var bankTransferIds = bankTransferIdsTask.Result;
            var recurringExpenseIds = recurringExpenseIdsTask.Result;

            if (categoryTypeIds.Length > 0) logger.LogWarning("Color has associated category types {@CategoryTypes}", categoryTypeIds);
            if (expenseIds.IsSuccess && expenseIds.Value!.Length > 0) logger.LogWarning("Color has associated expenses {@ExpenseIds}", expenseIds.Value);
            if (bankTransferIds.IsSuccess && bankTransferIds.Value!.Length > 0) logger.LogWarning("Color has associated bank transfers {@BankTransferIds}", bankTransferIds.Value);
            if (recurringExpenseIds.IsSuccess && recurringExpenseIds.Value!.Length > 0) logger.LogWarning("Color has associated recurring expenses {@RecurringExpenses}", recurringExpenseIds.Value);

            logger.LogInformation("Deleting color with id {ColorId}", colorDomain.Id);

            context.TColors.Remove(colorEntity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Color with id {ColorId} was successfully deleted", colorDomain.Id);

            var result = new Dictionary<DependencyType, int[]>
            {
                { DependencyType.CategoryType, categoryTypeIds },
                { DependencyType.Expense, expenseIds.Value! },
                { DependencyType.BankTransfer, bankTransferIds.Value! },
                { DependencyType.RecurringExpense, recurringExpenseIds.Value! }
            };
            return DeletionResult.Success("Color was successfully deleted", result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete color with id {ColorId}", colorDomain.Id);
            return DeletionResult.Failure(ErrorCode.DatabaseError, $"Failed to delete color: {e.Message}");
        }
    }

    public async Task<bool> IsColorNameAvailableAsync(string name, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Checking availability of color name {Name}", name);

        var available = !await context.TColors
            .AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);

        logger.LogInformation("Color name {Name} is {Available}", name, available ? "available" : "taken");

        return available;
    }

    public async Task<bool> IsColorHexadecimalCodeAvailableAsync(string hexadecimalCode, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Checking availability of color hexadecimal code {HexadecimalCode}", hexadecimalCode);

        var available = !await context.TColors
            .AnyAsync(c => c.HexadecimalColorCode.ToLower() == hexadecimalCode.ToLower(), cancellationToken);

        logger.LogInformation("Color hexadecimal code {HexadecimalCode} is {Available}", hexadecimalCode, available ? "available" : "taken");

        return available;
    }

    public async Task<PlaceDomain?> GetPlace(int placeId, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading place with id {PlaceId}", placeId);
        var place = await context.TPlaces
            .AsNoTracking()
            .ProjectToDomain()
            .FirstOrDefaultAsync(s => s.Id == placeId, cancellationToken);

        logger.LogInformation("Loaded place with id {PlaceId}, name {Name}", placeId, place?.Name);

        return place;
    }
}