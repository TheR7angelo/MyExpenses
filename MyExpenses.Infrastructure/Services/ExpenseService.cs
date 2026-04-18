using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Categories;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;

namespace MyExpenses.Infrastructure.Services;

public class ExpenseService(IExpenseRepository expenseRepository, IExpenseDtoDomainMapper mapper) : IExpenseService
{
    public async Task<IEnumerable<CategoryTypeDto>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await expenseRepository.GetAllCategoryTypesAsync(cancellationToken);
        return categories.Select(mapper.MapToDto);
    }

    public async Task<Result> AddCategoryTypeAsync(CategoryTypeDto categoryTypeDto, CancellationToken cancellationToken = default)
    {
        var accountType = mapper.MapToDomain(categoryTypeDto);
        return await expenseRepository.AddCategoryTypeAsync(accountType, cancellationToken);
    }
}