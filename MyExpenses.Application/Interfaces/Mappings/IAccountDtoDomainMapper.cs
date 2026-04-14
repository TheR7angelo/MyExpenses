using Domain.Models.Accounts;
using Domain.Models.Categories;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Dtos.Categories;

namespace MyExpenses.Application.Interfaces.Mappings;

public interface IAccountDtoDomainMapper
{
    /// <summary>
    /// Maps the TotalByAccountDomain object to a TotalByAccountDto object.
    /// </summary>
    /// <param name="src">The source TotalByAccountDomain object to map.</param>
    /// <returns>A TotalByAccountDto object that is mapped from the source object.</returns>
    public TotalByAccountDto MapToDto(TotalByAccountDomain src);

    /// <summary>
    /// Maps the AccountDomain object to an AccountDto object.
    /// </summary>
    /// <param name="src">The source AccountDomain object to map.</param>
    /// <returns>An AccountDto object that is mapped from the source object.</returns>
    public AccountDto MapToDto(AccountDomain src);

    /// <summary>
    /// Maps the AccountDto object to an AccountDomain object.
    /// </summary>
    /// <param name="src">The source AccountDto object to map.</param>
    /// <returns>An AccountDomain object that is mapped from the source object.</returns>
    public AccountDomain MapToDomain(AccountDto src);

    /// <summary>
    /// Maps the CurrencyDomain object to a CurrencyDto object.
    /// </summary>
    /// <param name="src">The source CurrencyDomain object to map.</param>
    /// <returns>A CurrencyDto object that is mapped from the source object.</returns>
    public CurrencyDto MapToDto(CurrencyDomain src);

    /// <summary>
    /// Maps the AccountTypeDomain object to an AccountTypeDto object.
    /// </summary>
    /// <param name="src">The source AccountTypeDomain object to map.</param>
    /// <returns>An AccountTypeDto object that is mapped from the source object.</returns>
    public AccountTypeDto MapToDto(AccountTypeDomain src);

    /// <summary>
    /// Maps the AccountTypeDto object to an AccountTypeDomain object.
    /// </summary>
    /// <param name="src">The source AccountTypeDto object to map.</param>
    /// <returns>An AccountTypeDomain object that is mapped from the source object.</returns>
    public AccountTypeDomain MapToDomain(AccountTypeDto src);

    /// <summary>
    /// Maps the CategoryTypeDto object to a CategoryTypeDomain object.
    /// </summary>
    /// <param name="src">The source CategoryTypeDto object to map.</param>
    /// <returns>A CategoryTypeDomain object that is mapped from the source object.</returns>
    public CategoryTypeDomain MapToDomain(CategoryTypeDto src);
}