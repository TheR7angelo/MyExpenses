using Domain.Models.Dependencies;
using Domain.Models.Validation;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Services;

public class ExpensePresentationService(IExpenseService categoryService, IExpenseDtoViewModelMapper mapper,
    IExpenseValidationRepository expenseValidationRepository,
    ILogger<ExpensePresentationService> logger) : IExpensePresentationService
{
    public async Task<IEnumerable<CategoryTypeViewModel>> GetAllCategoryTypeViewModelAsync(CancellationToken cancellationToken = default)
    {
        var categoryTypes = await categoryService.GetAllCategoryTypesAsync(cancellationToken);
        return categoryTypes.Select(mapper.MapToViewModel);
    }

    public async Task<bool> IsCategoryTypeNameAvailableAsync(string input, CancellationToken cancellationToken = default)
    {
        using var scope = logger.BeginScope("Checking category type name availability. Input={Input}", input);

        logger.LogInformation("Starting validation for category type name availability");

        try
        {
            var alreadyExists = await expenseValidationRepository.IsCategoryTypeNameAlreadyExistAsync(
                input, cancellationToken);

            if (alreadyExists)
            {
                logger.LogInformation("Category type name is already used");
                return false;
            }

            logger.LogInformation("Category type name is available");
            return true;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Validation was canceled");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while checking category type name availability");
            throw;
        }
    }

    public Task<Result> AddCategoryType(CategoryTypeViewModel newCategoryType, CancellationToken cancellationToken = default)
    {
        var categoryTypeDto = mapper.MapToDto(newCategoryType);
        return categoryService.AddCategoryTypeAsync(categoryTypeDto, cancellationToken);
    }
}