using Domain.Models.Accounts;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.ViewModels.Accounts;

namespace MyExpenses.Application.Mappings.Interfaces;

public interface IAccountDtoDomainMapper
{
    public TotalByAccountDto MapToDto(TotalByAccountDomain src);
    public TotalByAccountDomain MapToDomain(TotalByAccountDto src);
}

public interface IAccountDtoViewModelMapper
{
    public TotalByAccountViewModel MapToViewModel(TotalByAccountDto src);
    public TotalByAccountDto MapToDomain(TotalByAccountViewModel src);
}